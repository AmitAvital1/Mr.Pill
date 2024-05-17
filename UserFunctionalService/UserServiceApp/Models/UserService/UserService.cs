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
     private readonly string _baseUrlMOHservice;
     
    public UserService(IHttpClientFactory httpClientFactory, AppDbContext dbContext, ILogger<UserService> logger)
     {
        _httpClient = httpClientFactory.CreateClient();
        _dbContext = dbContext;
        _logger = logger;
        _baseUrlMOHservice = "http://localhost:5032/pill-details";
     }

     public void SaveMassageToManagerHouseToAddNewUser(LoginComunicationDWrapper loginComunicationDWrapper)
     {
        // the login service ensure that the phone number is a phone number of the manager
        int HouseId = getTheHouseIdByTheManagerPhoneNumber(loginComunicationDWrapper.ManagerPhone);
        int SenderPhoneNumber = loginComunicationDWrapper.SenderPhoneNumber;
        bool IsHandle = false;

        addNewRequestToTheDb(HouseId,SenderPhoneNumber,IsHandle, loginComunicationDWrapper.MergeToNewHouse);
     }

     private void addNewRequestToTheDb(int i_HouseId, int i_SenderPhoneNumber, bool i_IsHandle, bool i_MergeToNewHouse)
     {
        var request = new HouseRequest
        {
            HouseId = i_HouseId,
            SenderPhoneNumber = i_SenderPhoneNumber.ToString(),
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
        bool isMedicationExist = checkIfTheMedicationExistInDb(medicationBarcode);
        
        if (isMedicationExist)
        {
            addToUserNewMedication(phoneNumber, medicationBarcode, privatcy);
        }
        else
        {
           MedicationDTO? medicationDto  = await sendARequestToMinistryOfHealthService(medicationBarcode);

           if (medicationDto != null)
           {
                addToUserNewMedication(phoneNumber, medicationBarcode, privatcy);
                return true;
           }

           return false;
        }

        return true;
    }

    private void addToUserNewMedication (int phoneNumber, string medicationBarcode, bool privatcy)   
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
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
        var phoneNumberClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "PhoneNumber");
       
        if (phoneNumberClaim == null || string.IsNullOrEmpty(phoneNumberClaim.Value))
        {
            throw new InvalidOperationException("Phone number claim not found in token");
        }

        return phoneNumberClaim?.Value!;
    }    

    private async Task<MedicationDTO?> sendARequestToMinistryOfHealthService(string medicationBarcode)
    {
        MedicationDTO? medicationDTO = null;

        try
        {
           HttpResponseMessage response = await _httpClient.GetAsync($"{_baseUrlMOHservice}/{medicationBarcode}");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                medicationDTO = JsonConvert.DeserializeObject<MedicationDTO>(responseContent);

                if (medicationDTO != null)
                {
                    insertMedicationToMedicationRepositoryTable(medicationDTO, medicationBarcode);
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

    private void insertMedicationToMedicationRepositoryTable(MedicationDTO medicationDTO, string medicationBarcode)
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

    private bool checkIfTheMedicationExistInDb(string medicationBarcode)
    {
        var medication = _dbContext?.MedicationRepos.FirstOrDefault(m => m.Barcode == medicationBarcode);
        
        if (medication == null)
        {
            return false;
        }

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
            user.Medications = user?.Medications?.Where(m => m.IsPrivate == privacyStatus).ToList();
            
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
                    .WithIsPrivate(convertEnumToEnumDto(m.IsPrivate));

                return medicationDTOBuilder.Build();
            }) ?? Enumerable.Empty<MedicationDTO>();
       }
       catch (Exception ex)
       {
        _logger.LogError(ex, "An error occurred while fetching user.");
        return Enumerable.Empty<MedicationDTO>();
       }
    }

    public async Task<MedicationDTO> GetMedicationByBarcode(string medicationBarcode)
    {
        var medication = _dbContext.MedicationRepos.FirstOrDefault(r => r.Barcode == medicationBarcode);
        if (medication == null)
        {
           MedicationDTO? medicationDTO = await sendARequestToMinistryOfHealthService(medicationBarcode)!;
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

    private PrivacyStatusDTO convertEnumToEnumDto(PrivacyStatus privacyStatus)
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
            .Where(houseRequest => houseRequest.Id == houseId && houseRequest.IsHandle == false)
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
         var isManager = _dbContext.Houses.Any(house => house.Manager != null && house.Manager.PhoneNumber == phoneNumber);
         return isManager;
    }

    private IEnumerable<UserDTO> getUsersForHouseRequests(IEnumerable<HouseRequestDTO> houseRequests)
    {
        var phoneNumbers = houseRequests.Select(hr => hr.SenderPhoneNumber).ToList();

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

    public void UpdateMedication()
    {

    }

    public void DeleteMedication()
    {
        
    }

}