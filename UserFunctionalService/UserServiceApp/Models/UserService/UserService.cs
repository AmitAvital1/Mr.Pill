using Microsoft.EntityFrameworkCore;
using MrPill.DTOs.DTOs;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace UserServiceApp.Models.UserService;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _lockSemaphoreSlimForCreateNewMedication = new(1, 1);
    private static readonly object _lockForUpdateMedication = new();
    private readonly string _baseUrlMOHservice;
    public static string mohServiceUrl = "http://mohservice:8080/moh-service/pill-details";

    public UserService(IHttpClientFactory httpClientFactory, AppDbContext dbContext, ILogger<UserService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dbContext = dbContext;
        _logger = logger;
        _baseUrlMOHservice = mohServiceUrl;
    }

    public bool NameAlreadyExistInMyInventory(string Name, int phoneNumber)
    {
        _logger.LogInformation( "Checking if medicine cabinet name '{Name}' already exists for user with phone number {PhoneNumber}.", 
                    Name, phoneNumber
        );

        var user = _dbContext?.Users?
            .Include(u => u.MedicineCabinetUsersList!)
            ?.ThenInclude(mcu => mcu.MedicineCabinet)
            .ThenInclude(mc => mc.Medications)
            .FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            _logger.LogWarning("User with phone number {PhoneNumber} not found when checking for medicine cabinet name '{Name}'.", 
                phoneNumber, Name
            );

            return false;
        }

        var cabinetExists = user.MedicineCabinetUsersList?
            .Any(mcu => mcu.MedicineCabinet.MedicineCabinetName.Equals(Name, StringComparison.OrdinalIgnoreCase)) ?? false;

        if (!cabinetExists)
        {
            _logger.LogInformation("The medicine cabinet name '{Name}' is available for user with phone number {PhoneNumber}.", Name, phoneNumber);
            return false;
        }

        _logger.LogInformation("The medicine cabinet name '{Name}' already exists for user with phone number {PhoneNumber}.", Name, phoneNumber);
        return true;
    }

    public void CreateNewMedicineCabinet(string name, int phoneNumber)
    {
        try
        {
            var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

            CheckIfUserExist(user, phoneNumber);

            var newMedicineCabinet = new MedicineCabinet
            {
                MedicineCabinetName = name,
                CreatorId = user!.UserId,  
                Creator = user 
            };

            if (user.MedicineCabinetUsersList == null)
            {
                user.MedicineCabinetUsersList = new List<MedicineCabinetUsers>();
            }

            var newMedicineCabinetUser = new MedicineCabinetUsers
            {
                UserId = user.UserId,
                User = user,
                MedicineCabinet = newMedicineCabinet,
                MedicineCabinetId = newMedicineCabinet.Id
            };

            user.MedicineCabinetUsersList.Add(newMedicineCabinetUser);
            _dbContext!.MedicineCabinets.Add(newMedicineCabinet);
            _dbContext.SaveChanges();

            _logger.LogInformation(
                "Successfully created a new medicine cabinet with name '{Name}' for user with phone number {PhoneNumber}.", 
                name, 
                phoneNumber
            );
        }
        catch (Exception ex)
        {
            _logger.LogError (
                ex, 
                "An error occurred while creating a new medicine cabinet with name '{Name}' for user with phone number {PhoneNumber}.", 
                name, 
                phoneNumber
            );

            throw new InvalidOperationException (
                $"An error occurred while creating the medicine cabinet '{name}' for user with phone number {phoneNumber}.", 
                ex
            );
        }
    }

    private void CheckIfUserExist(User? user, int phoneNumber)
    {
        if (user == null)
        {
            _logger.LogWarning("User with phone number {PhoneNumber} does not exist.", phoneNumber);
            throw new InvalidOperationException($"User with phone number {phoneNumber} does not exist.");
        }
    }

    public void SaveMassageToManagerHouseToAddNewUser(LoginComunicationDWrapper loginComunicationDWrapper)
    {
        // the login service ensure that the phone number is a phone number of the manager

         _logger.LogInformation(
            "Starting to process the request to add a new user to the manager's house. ManagerPhone: {SourcePhoneNumber}, " +
            "SenderPhoneNumber: {TargetPhoneNumber}, MedicineCabinetName: {MedicineCabinetName}",
            loginComunicationDWrapper.SourcePhoneNumber, loginComunicationDWrapper.TargetPhoneNumber, loginComunicationDWrapper.MedicineCabinetName
        );

        int senderPhoneNumber = loginComunicationDWrapper.SourcePhoneNumber;
        int targetPhoneNumber = loginComunicationDWrapper.TargetPhoneNumber;
        string cabinetName = loginComunicationDWrapper.MedicineCabinetName;
        bool isHandle = false;

        addNewRequestToTheDb(senderPhoneNumber,targetPhoneNumber, cabinetName, isHandle);
    }

    private void addNewRequestToTheDb(int senderPhoneNumber, int targetPhoneNumber, string cabinetName, bool isHandle)
    {
        var currentTime = DateTime.Now;

        _logger.LogInformation(
            "Adding new request to the database at {CurrentTime}. " +
            "SenderPhoneNumber: {SenderPhoneNumber}, TargetPhoneNumber: {TargetPhoneNumber}, " +
            "CabinetName: {CabinetName}, IsHandle: {IsHandle}",
            currentTime, senderPhoneNumber, targetPhoneNumber, cabinetName, isHandle
        );

        var request = new CabinetRequest
        {
            SenderPhoneNumber = senderPhoneNumber.ToString(),
            TargetPhoneNumber = targetPhoneNumber.ToString(),
            CabinetName = cabinetName,
            IsHandle = isHandle,
            IsApprove = false, 
            IsSenderSeen = false, 
            DateStart = DateTime.Now,
            DateEnd = DateTime.MaxValue 
        };

        _dbContext.CabinetRequests.Add(request);
        _dbContext.SaveChanges();
    }

    public bool IsUserExistInDb(int PhoneNumber)
    {
        if (_dbContext.Users != null)
        {
            return _dbContext.Users.Any(user => user.PhoneNumber == PhoneNumber);
        }

        return false;
    }

    public int GetUserPhoneNumber(string token)
    {
        string phoneNumber = getPhoneNumberFromToken(token);

        return int.Parse(phoneNumber);
    }

    public async Task<bool> CreateNewMedication(string medicationBarcode, int phoneNumber, bool privatcy, string medicineCabinetName)
    {
        // We can't use `lock` with `await` because the thread releases the CPU but still holds the lock.
        // This can lead to deadlocks if another thread tries to acquire the same lock concurrently.

        // SemaphoreSlim is a better choice because it allows the thread to release the lock while waiting asynchronously.
        // The thread acquires the SemaphoreSlim before the critical section (checking medication existence).
        // After acquiring the SemaphoreSlim, it can release the lock and wait for the asynchronous operation (if any) to complete.
        // The continuation task (code after `await`) is responsible for completing its work and releasing the SemaphoreSlim.
        // This ensures other threads can acquire the SemaphoreSlim and access the critical section when available.
        
        await _lockSemaphoreSlimForCreateNewMedication.WaitAsync().ConfigureAwait(true);
        
        try
        {
            bool isMedicationExist = CheckIfTheMedicationExistInDb(medicationBarcode);

            if (isMedicationExist)
            {
                _logger.LogInformation(
                    "Medication with barcode {MedicationBarcode} already exists. " +
                    "Adding to user {PhoneNumber}.", 
                    medicationBarcode, 
                    phoneNumber
                );

                AddToUserNewMedication(phoneNumber, medicationBarcode, privatcy, medicineCabinetName);
            }
            else
            {
                MedicationDTO? medicationDto = await SendARequestToMinistryOfHealthService(medicationBarcode);

                if (medicationDto != null)
                {
                    AddToUserNewMedication(phoneNumber, medicationBarcode, privatcy, medicineCabinetName);
                    return true;
                }

                _logger.LogWarning(
                    "No MedicationDTO received from Ministry of Health for barcode {MedicationBarcode}. " +
                    "The request returned null.", 
                    medicationBarcode
                );

                return false;
            }

            return true;
        }
        finally
        {
            _lockSemaphoreSlimForCreateNewMedication.Release();
        }
    }

    private void AddToUserNewMedication(int phoneNumber, string medicationBarcode, bool privacy, string medicineCabinetName)
    {
        try
        {
            var user = GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                _logger.LogError(
                    "User not found to by phone number {PhoneNumber}"
                    ,phoneNumber
                );

                throw new InvalidOperationException($"Medication not found for barcode {medicationBarcode}.");
            }

            var medication = GetMedicationByBarcodeWithoutReturnADto(medicationBarcode);
            if (medication == null)
            {
                
                _logger.LogError(
                    "Error getting medication by barcode {medicationBarcode}"
                    ,medicationBarcode
                );

                throw new InvalidOperationException($"Medication not found for barcode {medicationBarcode}.");
            }

            var medicineCabinet = GetMedicineCabinetByName(user, medicineCabinetName);
            if (medicineCabinet == null)
            {
                _logger.LogError(
                    "Error getting medicine cabinet by name - {medicineCabinetName}"
                    ,medicineCabinetName
                );
                
                throw new InvalidOperationException($"Medicine cabinet not found for name {medicineCabinetName}.");
            }

            AddMedicationToCabinet(user, medication, privacy, medicineCabinet);

            _logger.LogInformation(
                "Successfully added medication with barcode '{Barcode}' to the medicine cabinet '{MedicineCabinetName}' " +
                "for user with phone number {PhoneNumber}.", 
                medicationBarcode, 
                medicineCabinetName, 
                phoneNumber
            );
        }
        catch (Exception ex)
        {
           _logger.LogError(
                ex, 
                "An error occurred while adding medication with barcode '{Barcode}' to the medicine cabinet '{MedicineCabinetName}' " +
                "for user with phone number {PhoneNumber}.", 
                medicationBarcode, 
                medicineCabinetName, 
                phoneNumber
            );

            throw new InvalidOperationException(
                $"Failed to add medication with barcode '{medicationBarcode}' to the medicine cabinet '{medicineCabinetName}' " +
                $"for user with phone number '{phoneNumber}'. See inner exception for details.", 
                ex
            );
        }
    }

    private User? GetUserByPhoneNumber(int phoneNumber)
    {
        var user = _dbContext?.Users?.SingleOrDefault(u => u.PhoneNumber == phoneNumber);
        
        if (user == null)
        {
            _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (this check was made by userService).", phoneNumber);
        }

        return user;
    }

    private MedicationRepo? GetMedicationByBarcodeWithoutReturnADto(string medicationBarcode)
    {
        var medication = _dbContext?.MedicationRepos.SingleOrDefault(m => m.Barcode == medicationBarcode);
        
        if (medication == null)
        {
            _logger.LogInformation("The medication with the barcode {medicationBarcode} does not exist in the database.", medicationBarcode);
        }

        return medication;
    }

    private MedicineCabinet? GetMedicineCabinetByName(User user, string medicineCabinetName)
    {
        var medicineCabinet = user.MedicineCabinetUsersList?
            .Select(mcu => mcu.MedicineCabinet)
            .SingleOrDefault(mc => mc.MedicineCabinetName.Equals(medicineCabinetName, StringComparison.OrdinalIgnoreCase));

        if (medicineCabinet == null)
        {
           _logger.LogInformation(
                "The medicine cabinet with the name '{MedicineCabinetName}' does not exist for user with phone number {PhoneNumber}.", 
                medicineCabinetName, 
                user.PhoneNumber
            );
        }

        return medicineCabinet;
    }

    private void AddMedicationToCabinet(User user, MedicationRepo medication, bool privacy, MedicineCabinet medicineCabinet)
    {
        var userMedication = new UserMedications
        {
            Barcode = medication.Barcode,
            Validity = DateTime.Now.AddMonths(6),
            Creator = user,
            MedicineCabinet = medicineCabinet,
            IsPrivate = privacy,
            NumberOfPills = medication.largestPackage,
            MedicationRepo = medication
        };

        _dbContext?.UserMedications.Add(userMedication);
        _dbContext?.SaveChanges();
    }

    public int GetPhoneNumberFromToken(string? token)
    {
        if (token == null)
        {
            return -1;
        }

        return int.Parse(getPhoneNumberFromToken(token));
    }

   private string getPhoneNumberFromToken(string token)
    {
        _logger.LogInformation("Extracting phone number from token: {Token}", token);

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
        var phoneNumberClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "PhoneNumber");

        if (phoneNumberClaim == null || string.IsNullOrEmpty(phoneNumberClaim.Value))
        {
            _logger.LogWarning("Phone number claim not found or is empty in the token.");
            throw new InvalidOperationException("Phone number claim not found in token");
        }

        _logger.LogInformation("Extracted phone number: {PhoneNumber}", phoneNumberClaim.Value);
        return phoneNumberClaim?.Value!;
    }

    private async Task<MedicationDTO?> SendARequestToMinistryOfHealthService(string medicationBarcode)
    {
        MedicationDTO? medicationDTO = null;

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_baseUrlMOHservice}/{medicationBarcode}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogDebug("No medication found in Moh");
                    return null;
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                medicationDTO = JsonConvert.DeserializeObject<MedicationDTO>(responseContent);

                if (medicationDTO != null)
                {
                    InsertMedicationToMedicationRepositoryTable(medicationDTO, medicationBarcode);
                }
                else
                {
                    _logger.LogError("Failed to fetch medication information. MedicationDTO is null.");
                }
            }
            else
            {
                _logger.LogError("Failed to fetch medication information. HTTP status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An error occurred while sending the HTTP request.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "An error occurred while deserializing the JSON response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
        }

        return medicationDTO;
    }

    private void InsertMedicationToMedicationRepositoryTable(MedicationDTO medicationDTO, string medicationBarcode)
    {
        var medicationRepo = new MedicationRepo
        {
            Barcode = medicationBarcode,
            DrugEnglishName = medicationDTO.EnglishName ?? "DefaultEnglishName",
            DrugHebrewName = medicationDTO.HebrewName ?? "DefaultHebrewName",
            EnglishDescription = medicationDTO.EnglishDescription,
            HebrewDescription = medicationDTO.HebrewDescription,
            ImagePath = medicationDTO.ImagePath
        };

        _dbContext.MedicationRepos.Add(medicationRepo);
        _dbContext.SaveChanges();
    }

    private bool CheckIfTheMedicationExistInDb(string medicationBarcode)
    {
        _logger.LogInformation("Checking if medication with barcode {MedicationBarcode} exists in the database.", medicationBarcode);

        var medication = _dbContext?.MedicationRepos
                .FirstOrDefault(m => m.Barcode == medicationBarcode);

        if (medication == null)
        {
            _logger.LogInformation("Medication with barcode {MedicationBarcode} does not exist in the database.", medicationBarcode);
            return false;
        }

        _logger.LogInformation("Medication with barcode {MedicationBarcode} found in the database.", medicationBarcode);
        return true;
    }

    public IEnumerable<MedicationDTO> GetAllMedicationByUserId(int phoneNumber, string medicineCabinetName)
    {
        try
        {
            var user = GetUserByPhoneNumber(phoneNumber);
          
            if (user == null)
            {
                _logger.LogError("User with phone number {PhoneNumber} not found.", phoneNumber);
                throw new Exception("User not found");
            }
          
            var medicineCabinet = GetMedicineCabinetByName(user, medicineCabinetName);
            
            if (medicineCabinet == null)
            {
                _logger.LogError(
                    "Medicine cabinet with name '{MedicineCabinetName}' not found for user with phone number {PhoneNumber}.", 
                    medicineCabinetName, 
                    phoneNumber
                );

                throw new Exception("Medicine cabinet not found");
            }

            var filteredMedications = FilterMedications(user, medicineCabinet);

            return ConvertMedicationsToDTOs(filteredMedications);

        }
       catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "An error occurred while getting medications for user with phone number {PhoneNumber}.", 
                phoneNumber
            );

            throw new InvalidOperationException(
                $"An error occurred while retrieving medications for the user with phone number {phoneNumber}.", 
                ex
            );
        }
    }

    public IEnumerable<MedicationDTO> GetAllMedication(int userPhoneNumber)
    {
        try
        {
            var user = _dbContext?.Users
                    ?.Include(u => u.MedicineCabinetUsersList!)
                        .ThenInclude(mcu => mcu.MedicineCabinet)
                            .ThenInclude(mc => mc.Medications)
                    .FirstOrDefault(u => u.PhoneNumber == userPhoneNumber);

            if (user == null)
            {
                _logger.LogError("User with phone number {PhoneNumber} not found.", userPhoneNumber);
                throw new Exception("User not found");
            }

            var medications = user.MedicineCabinetUsersList
                ?.Where(mcu => mcu.MedicineCabinet?.Medications != null)
                .SelectMany(mcu => mcu.MedicineCabinet.Medications ?? Enumerable.Empty<UserMedications>())
                .Where(m => !m.IsPrivate || m.CreatorId == user.UserId)
                .ToList();

            return ConvertMedicationsToDTOs(medications ?? new List<UserMedications>());

        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "An error occurred while getting medications for user with phone number {userPhoneNumber}.", 
                userPhoneNumber
            );

            throw new InvalidOperationException(
                $"An error occurred while retrieving medications for the user with phone number {userPhoneNumber}.", 
                ex
            );
        }
    }

    private IEnumerable<MedicationDTO> ConvertMedicationsToDTOs(IEnumerable<UserMedications> medications)
    {
        return medications.Select(m =>
        {
            var medicationDTOBuilder = MedicationDTO.Builder()
                .WithId(m.Id)
                .WithEnglishName(m.MedicationRepo.DrugEnglishName)
                .WithHebrewName(m.MedicationRepo.DrugHebrewName)
                .WithEnglishDescription(m.MedicationRepo.EnglishDescription)
                .WithHebrewDescription(m.MedicationRepo.HebrewDescription)
                .WithValidity(m.Validity)
                .WithUserId(m.CreatorId)
                .WithMedicationRepoId(m.MedicationRepoId)
                .WithMedicineCabinetName(m.MedicineCabinet.MedicineCabinetName)
                .WithImagePath(m.MedicationRepo.ImagePath)
                .WithIsPrivate(m.IsPrivate);

            return medicationDTOBuilder.Build();
        });
    }

    private List<UserMedications> FilterMedications(User user, MedicineCabinet medicineCabinet)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        if (medicineCabinet == null)
        {
            throw new ArgumentNullException(nameof(medicineCabinet), "Medicine cabinet cannot be null");
        }

        return medicineCabinet.Medications?
            .Where(m => !m.IsPrivate || (m.IsPrivate && m.CreatorId == user.UserId))
            .ToList() ?? new List<UserMedications>();
    }

    public IEnumerable<MedicineCabinetDTO> GetAllMedicineCabinets(int userPhoneNumer)
    {
        var user = GetUserByPhoneNumber(userPhoneNumer);
          
        if (user == null)
        {
            _logger.LogError("User with phone number {PhoneNumber} not found.", userPhoneNumer);
            throw new Exception("User not found");
        }

        var medicineCabinets = user.MedicineCabinetUsersList?
            .Select(mcu => mcu.MedicineCabinet)
            .ToList();

        if (medicineCabinets == null || !medicineCabinets.Any())
        {
            _logger.LogInformation("No medicine cabinets found for user with phone number {PhoneNumber}.", userPhoneNumer);
            return Enumerable.Empty<MedicineCabinetDTO>();
        }

        return medicineCabinets.Select(mc =>
        {
            return MedicineCabinetDTO.Builder()
                .WithId(mc.Id)
                .WithMedicineCabinetName(mc.MedicineCabinetName)
                .WithCreatorId(mc.CreatorId)
                .Build();
        }).ToList();
    }

    public async Task<MedicationDTO> GetMedicationByBarcode(string medicationBarcode)
    {
        var medication = _dbContext.MedicationRepos
                .FirstOrDefault(r => r.Barcode == medicationBarcode);
        
        if (medication == null)
        {
            MedicationDTO? medicationDTO = await SendARequestToMinistryOfHealthService(medicationBarcode)!; 
            return medicationDTO!;
        }
        
        var medicationDTOBuilder = MedicationDTO.Builder()
                        .WithId(medication.Id)
                        .WithEnglishName(medication.DrugEnglishName)
                        .WithHebrewName(medication.DrugHebrewName)
                        .WithEnglishDescription(medication.EnglishDescription)
                        .WithHebrewDescription(medication.HebrewDescription)
                        .Build();

        return medicationDTOBuilder;
    }

    public void DeleteMedication(int userPhoneNumber, int medicationId, string medicineCabinetName)
    {
        var user = GetUserByPhoneNumber(userPhoneNumber);
          
        if (user == null)
        {
            _logger.LogError("User with phone number {PhoneNumber} not found.", userPhoneNumber);
            throw new Exception("User not found");
        }
          
        var medicineCabinet = GetMedicineCabinetByName(user, medicineCabinetName);
        
        if (medicineCabinet == null)
        {
            _logger.LogError("Medicine cabinet with name '{MedicineCabinetName}' not found for user.", medicineCabinetName);
            throw new Exception("Medicine cabinet not found");
        }

        var medication = medicineCabinet.Medications
            ?.SingleOrDefault(m => m.Id == medicationId);

        if (medication == null)
        {
            _logger.LogError(
                "Medication with ID {MedicationId} not found in medicine cabinet '{MedicineCabinetName}' for user with phone number {PhoneNumber}.", 
                medicationId, 
                medicineCabinetName, 
                userPhoneNumber
            );

            throw new InvalidOperationException(
                $"Medication with ID {medicationId} not found in medicine cabinet '{medicineCabinetName}' for user with phone number {userPhoneNumber}"
            );
        }

        medicineCabinet?.Medications?.Remove(medication);
        _dbContext?.SaveChanges();
    }

    public void UpdateMedication( UpdateMedicationDTO updateMedication)
    {
        var medicineCabinet = _dbContext.MedicineCabinets
            .Include(mc => mc.Medications)
            .FirstOrDefault(mc => mc.Id == updateMedication.MedicationRepoId);

        if (medicineCabinet == null)
        {
            _logger.LogWarning(
                "Medicine cabinet with ID {MedicineCabinetId} not found.",
                updateMedication.MedicationRepoId
            );

            throw new Exception("Medicine cabinet not found.");
        }

        var medication = medicineCabinet?.Medications
            ?.FirstOrDefault(m => m.Id == updateMedication.MedicationId);

        if (medication == null)
        {
             _logger.LogWarning(
                "Medication with ID {MedicationId} not found in Medicine Cabinet ID {MedicineCabinetId}.",
                updateMedication.MedicationId,
                updateMedication.MedicationId
            );

            throw new Exception("Medication not found in the specified medicine cabinet.");
        }

        lock (_lockForUpdateMedication)
        {

            if (medication.NumberOfPills > 0)
            {
                medication.NumberOfPills -= 1;
            }
            else
            {
                throw new Exception("No pills left to decrease.");
            }

            _dbContext.SaveChanges();
        }

        _logger.LogInformation(
            "Database updated successfully for MedicationId {MedicationId} in Medicine Cabinet ID {MedicineCabinetId}.",
            medication.Id,
            medicineCabinet?.Id
        );  
    }
}