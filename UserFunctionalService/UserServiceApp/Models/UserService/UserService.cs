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
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _baseUrlMOHservice;
    public static string mohServiceUrl = "http://mohservice:8080/moh-service/pill-details";

    public UserService(IHttpClientFactory httpClientFactory, AppDbContext dbContext, ILogger<UserService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dbContext = dbContext;
        _logger = logger;
        _baseUrlMOHservice = mohServiceUrl;
    }

    public void SaveMassageToManagerHouseToAddNewUser(LoginComunicationDWrapper loginComunicationDWrapper)
    {
        // the login service ensure that the phone number is a phone number of the manager

         _logger.LogInformation("Starting to process the request to add a new user to the manager's house. ManagerPhone: {ManagerPhone}, " +
                       "SenderPhoneNumber: {SenderPhoneNumber}, MergeToNewHouse: {MergeToNewHouse}",
                       loginComunicationDWrapper.ManagerPhone, 
                       loginComunicationDWrapper.SenderPhoneNumber, 
                       loginComunicationDWrapper.MergeToNewHouse);


        int HouseId = getTheHouseIdByTheManagerPhoneNumber(loginComunicationDWrapper.ManagerPhone);
        int senderPhoneNumber = loginComunicationDWrapper.SenderPhoneNumber;
        int targetPhoneNumber = loginComunicationDWrapper.ManagerPhone;
        bool IsHandle = false;

        addNewRequestToTheDb(HouseId, senderPhoneNumber, IsHandle, loginComunicationDWrapper.MergeToNewHouse, targetPhoneNumber);
    }

    private void addNewRequestToTheDb(int i_HouseId, int i_SenderPhoneNumber, bool i_IsHandle, bool i_MergeToNewHouse, int i_TargetPhoneNumber)
    {
        var currentTime = DateTime.Now;

        _logger.LogInformation(
        "Adding new request to the database at {CurrentTime}. " +
        "HouseId: {HouseId}, SenderPhoneNumber: {SenderPhoneNumber}, " +
        "IsHandle: {IsHandle}, MergeToNewHouse: {MergeToNewHouse}",
        currentTime, i_HouseId, i_SenderPhoneNumber, i_IsHandle, i_MergeToNewHouse);

        var request = new HouseRequest
        {
            HouseId = i_HouseId,
            SenderPhoneNumber = i_SenderPhoneNumber.ToString(),
            TargetPhoneNumber = i_TargetPhoneNumber.ToString(),
            IsHandle = i_IsHandle,
            MergeToNewHouse = i_MergeToNewHouse,
            DateStart = DateTime.Now
        };

        _dbContext.HouseRequests.Add(request);
        _dbContext.SaveChanges();
    }

    private int getTheHouseIdByTheManagerPhoneNumber(int managerPhone)
    {
        var manager = _dbContext?.Users?.FirstOrDefault(u => u.PhoneNumber == managerPhone);
        
        if (manager != null)
        {
            return manager.HouseId;
        }

        return -1;
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

    public async Task<bool> CreateNewMedication(string medicationBarcode, int phoneNumber, bool privatcy)
    {
        // We can't use `lock` with `await` because the thread releases the CPU but still holds the lock.
        // This can lead to deadlocks if another thread tries to acquire the same lock concurrently.

        // SemaphoreSlim is a better choice because it allows the thread to release the lock while waiting asynchronously.
        // The thread acquires the SemaphoreSlim before the critical section (checking medication existence).
        // After acquiring the SemaphoreSlim, it can release the lock and wait for the asynchronous operation (if any) to complete.
        // The continuation task (code after `await`) is responsible for completing its work and releasing the SemaphoreSlim.
        // This ensures other threads can acquire the SemaphoreSlim and access the critical section when available.
 
        await _lock.WaitAsync().ConfigureAwait(true);
        
        try
        {
            bool isMedicationExist = CheckIfTheMedicationExistInDb(medicationBarcode);

            if (isMedicationExist)
            {
                _logger.LogInformation("Medication with barcode {MedicationBarcode} already exists. Adding to user {PhoneNumber}. ", medicationBarcode, phoneNumber);
                AddToUserNewMedication(phoneNumber, medicationBarcode, privatcy);
            }
            else
            {
                MedicationDTO? medicationDto = await SendARequestToMinistryOfHealthService(medicationBarcode);

                if (medicationDto != null)
                {
                    AddToUserNewMedication(phoneNumber, medicationBarcode, privatcy);
                    return true;
                }

                _logger.LogWarning("No MedicationDTO received from Ministry of Health for barcode {MedicationBarcode}. The request returned null.", medicationBarcode);
                return false;
            }

            return true;
        }
        finally
        {
            _lock.Release();
        }

    }

    private void AddToUserNewMedication(int phoneNumber, string medicationBarcode, bool privatcy)
    {
        var user = _dbContext?.Users?.SingleOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (this check was made by userService)", phoneNumber);
            return;
        }

        var medication = _dbContext?.MedicationRepos.SingleOrDefault(m => m.Barcode == medicationBarcode);
        
        if (medication == null)
        {
            _logger.LogInformation("The medication with the barcode {medicationBarcode} does not exist in the database", medicationBarcode);
            return;
        }

        var userMedication = new UserMedications
        {
            Barcode = medication.Barcode,
            Validity = DateTime.Now,
            User = user,
            IsPrivate = convertBooleanToPrivacyStatus(privatcy),
            MedicationRepo = medication
        };

        _dbContext?.UserMedications.Add(userMedication);
        _dbContext?.SaveChanges();
    }

    private PrivacyStatus convertBooleanToPrivacyStatus(bool isPrivate)
    {
        return isPrivate ? PrivacyStatus.PrivateMedications : PrivacyStatus.PublicMedications;
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
                if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
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


    public IEnumerable<MedicationDTO> GetAllMedicationByUserId(int phoneNumber, PrivacyStatus privacyStatus)
    {
        try
        {
            var user = _dbContext?.Users
                ?.Include(u => u.Medications)
                .FirstOrDefault(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                _logger.LogError("User with phone number {PhoneNumber} not found.", phoneNumber);
                throw new Exception("User not found");
            }
          
            user.Medications = FilterMedications(user, privacyStatus);

            return user?.Medications?.Select(m =>
            {
                var medicationDTOBuilder = MedicationDTO.Builder()
                    .WithId(m.Id)
                    .WithEnglishName(m.MedicationRepo.DrugEnglishName)
                    .WithHebrewName(m.MedicationRepo.DrugHebrewName)
                    .WithEnglishDescription(m.MedicationRepo.EnglishDescription)
                    .WithHebrewDescription(m.MedicationRepo.HebrewDescription)
                    .WithValidity(m.Validity)
                    .WithUserId(m.UserId)
                    .WithMedicationRepoId(m.MedicationRepoId)
                    .WithImagePath(m.MedicationRepo.ImagePath)
                    .WithIsPrivate(ConvertEnumToEnumDto(m.IsPrivate));

                return medicationDTOBuilder.Build();
            }) ?? Enumerable.Empty<MedicationDTO>();  
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting medications for user with phone number {PhoneNumber}.", phoneNumber);
            return Enumerable.Empty<MedicationDTO>();
        }
    }

    private List<UserMedications> FilterMedications(User user, PrivacyStatus privacyStatus)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        if (privacyStatus == PrivacyStatus.PrivateMedications)
        {
            return user.Medications?.Where(m => m.IsPrivate == PrivacyStatus.PrivateMedications).ToList()!;
        }
        
        return user.Medications?.ToList()!;
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

    private PrivacyStatusDTO ConvertEnumToEnumDto(PrivacyStatus privacyStatus)
    {
        return (PrivacyStatusDTO)privacyStatus;
    }

    public IEnumerable<UserDTO> GetAllUsersThatWantToBePartOfMyHome(int userPhoneNumer)
    {
        var House = _dbContext.Houses.FirstOrDefault(r => r.Manager!.PhoneNumber == userPhoneNumer);

        if (House == null)
        {
            _logger.LogError("An error occurred while fetching Notification.");
            return Enumerable.Empty<UserDTO>();
        }

        var houseRequests = getHouseRequests(House.Id);
        var phoneNumbers = houseRequests.Select(hr => hr.SenderPhoneNumber).ToList();
        var users = getUsersForHouseRequests(houseRequests);

        return users ?? Enumerable.Empty<UserDTO>();
    }

    private IEnumerable<HouseRequestDTO> getHouseRequests(int houseId)
    {
        return _dbContext.HouseRequests
            .Where(houseRequest => 
                    houseRequest.Id == houseId && houseRequest.IsHandle == false)
                .Select(houseRequest => HouseRequestDTO.Builder()
                    .WithId(houseRequest.Id)
                    .WithHouseId(houseRequest.HouseId)
                    .WithSenderPhoneNumber(houseRequest.SenderPhoneNumber)
                    .WithIsHandled(houseRequest.IsHandle)
                    .WithIsApproved(houseRequest.IsApprove)
                    .WithIsSenderSeen(houseRequest.IsSenderSeen)
                    .WithMergeToNewHouse(houseRequest.MergeToNewHouse)
                    .WithDateStart(houseRequest.DateStart)
                    .WithDateEnd(houseRequest.DateEnd)
                    .Build()
            );
    }

    public bool IsManager(int phoneNumber)
    {
        var isManager = _dbContext.Houses
                .Any(house => house.Manager != null && house.Manager.PhoneNumber == phoneNumber);

        return isManager;
    }

    private IEnumerable<UserDTO> getUsersForHouseRequests(IEnumerable<HouseRequestDTO> houseRequests)
    {
        var phoneNumbers = houseRequests
                .Select(hr => hr.SenderPhoneNumber).ToList();

        return _dbContext.Users
            ?.Where(user => phoneNumbers.Contains(user.PhoneNumber.ToString()))
                .Select(user => UserDTO.Builder()
                    .WithFirstName(user.FirstName)
                    .WithLastName(user.LastName)
                    .WithPhoneNumber(user.PhoneNumber.ToString())
                    .Build()
                )
            ?? Enumerable.Empty<UserDTO>();
    }

    public void DeleteMedication(int userPhoneNumber, int medicationId)
    {
        var user = _dbContext?.Users
                ?.Include(u => u.Medications)
                .SingleOrDefault(u => u.PhoneNumber == userPhoneNumber);

        if (user == null)
        {
            _logger.LogError("User with phone number {PhoneNumber} not found", userPhoneNumber);
            throw new InvalidOperationException($"User with phone number {userPhoneNumber} not found");
        }

        var medication = user.Medications
                ?.SingleOrDefault(m => m.Id == medicationId);

        if (medication == null)
        {
            _logger.LogError("Medication with ID {MedicationId} not found for user with phone number {PhoneNumber}", medicationId, userPhoneNumber);
            throw new InvalidOperationException($"Medication with ID {medicationId} not found for user with phone number {userPhoneNumber}");
        }

        user?.Medications?.Remove(medication);
        _dbContext?.SaveChanges();
    }

    public void InviteMemberToJoindMyHouse(int managerPhoneNumber, int phoneNumber)
    {
        // first we need to create a new notification to the user to we need to send a request to the pushNotificationService
        _logger.LogInformation("Inviting member with phone number {PhoneNumber} to join the house managed by {ManagerPhoneNumber}.", phoneNumber, managerPhoneNumber);
        try
        {
            int houseId = GetHouseIdMyManagerPhoneNumber(managerPhoneNumber);

            _logger.LogInformation("Successfully retrieved HouseId {HouseId} for manager {ManagerPhoneNumber}. Proceeding to create a house request.", houseId, managerPhoneNumber);

            HouseRequest request = new HouseRequest
            {
                HouseId = houseId,
                SenderPhoneNumber = managerPhoneNumber.ToString(),
                TargetPhoneNumber = phoneNumber.ToString(),
                IsHandle = false,
                IsApprove = false,
                IsSenderSeen = false,
                IsManagerHouseSendRequest = true,
                MergeToNewHouse = false,
                DateStart = DateTime.Now, 
                DateEnd = DateTime.Now.AddDays(7) 
            };

            _dbContext.HouseRequests.Add(request);
            _dbContext.SaveChanges();

            _logger.LogInformation("House request successfully created and saved for member {PhoneNumber} to join house {HouseId}.", phoneNumber, houseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error occurred when try to send a request to new member. {ex.Message}");
            throw;
        }
    }

    private int GetHouseIdMyManagerPhoneNumber(int managerPhoneNumber)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve HouseId for manager with phone number {ManagerPhoneNumber}.", managerPhoneNumber);

            var houseId = _dbContext?.Users
                    ?.Where(u => u.PhoneNumber == managerPhoneNumber)
                    .Select(u => u.HouseId)
                    .FirstOrDefault();
            
            if (houseId == null)
            {
                _logger.LogWarning("No house found for manager with phone number {ManagerPhoneNumber}.", managerPhoneNumber);
                throw new InvalidOperationException($"No house found for manager with phone number {managerPhoneNumber}");
            }

            _logger.LogInformation("Successfully retrieved HouseId {HouseId} for manager with phone number {ManagerPhoneNumber}.", houseId, managerPhoneNumber);

            return houseId.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while fetching HouseId for manager with phone number {managerPhoneNumber}.");   
            throw;
        }
    }

    public void UpdateMedication(MedicationDTO medicationDTO)
    {
        
    }

}