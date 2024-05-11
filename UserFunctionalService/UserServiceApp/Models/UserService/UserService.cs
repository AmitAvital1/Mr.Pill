using MrPill.DTOs.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace UserServiceApp.Models.UserService;

public class UserService : IUserService
{
     private readonly HttpClient _httpClient;
     private readonly AppDbContext _dbContext;
     private readonly ILogger _logger;
     
    public UserService(IHttpClientFactory httpClientFactory, AppDbContext dbContext, ILogger<UserService> logger)
     {
        _httpClient = httpClientFactory.CreateClient();
        _dbContext = dbContext;
        _logger = logger;
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

     public bool isUserExistInDb(int PhoneNumber)
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

    public bool CreateNewMedication(string medicationName, int phoneNumber)
    {
        // this function need to get the userId
        // if the medication exist we add to user db
        // else we need to send a request to the service of the medication
        bool isMedicationExist = checkIfTheMedicationExistInDb(medicationName);
        bool isAddSuccess = false;

        if (isMedicationExist)
        {
            addToUserNewMedication(phoneNumber, medicationName);
        }
        else
        {
           MedicationDTO medicationDto  = sendARequestToMinistryOfHealthService(ref isAddSuccess);

           if (isAddSuccess)
           {
            //same function like the first if
           }
        }

        return isAddSuccess;
    }

    private void addToUserNewMedication (int phoneNumber, string medicationName)   
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.PhoneNumber == phoneNumber);
        if (user == null)
        {
            _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (this check was made by userService)", phoneNumber);
             return;
        }

        var medication = _dbContext.MedicationRepos.SingleOrDefault(m => m.DrugEnglishName == medicationName);
        if (medication == null)
        {
            _logger.LogInformation("The medication {medicationName} does not exist in the database", medicationName);
            return;
        }

        var userMedication = new UserMedications
        {
            Barcode = medication.Barcode,
            Validity = DateTime.Now,
            User = user,
            MedicationRepo = medication
        };

        _dbContext.UserMedications.Add(userMedication);
        _dbContext.SaveChanges();
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
    

    private MedicationDTO sendARequestToMinistryOfHealthService(ref bool isAddSuccess)
    {
        throw new NotImplementedException();
    }

    private bool checkIfTheMedicationExistInDb(string medicationName)
    {
        return true;
    }

    public void UpdateMedication()
    {

    }

    public void DeleteMedication()
    {
        
    }

    public IEnumerable<MedicationDTO> GetAllMedicationByUserId(int userId)
    {
        return null;
    }

    public MedicationDTO GetMedicationByName(string medicationName)
    {
        return null;
    }

}