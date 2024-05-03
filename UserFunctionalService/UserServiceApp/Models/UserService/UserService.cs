using MrPill.DTOs.DTOs;
using RabbitMQ.Client;
using System.Text;

namespace UserServiceApp.Models.UserService;

public class UserService : IUserService
{
     private readonly HttpClient _httpClient;
     private readonly AppDbContext _dbContext;
     private readonly ILogger _logger;

     public UserService(IHttpClientFactory httpClientFactory, AppDbContext dbContext, ILogger<UserService>logger)
     {
        _httpClient = httpClientFactory.CreateClient();
        _dbContext = dbContext;
        _logger = logger;
     }

    public bool CreateNewMedication(string medicationName)
    {
        // this function need to get the userId
        // if the medication exist we add to user db
        // else we need to send a request to the service of the medication
        bool isMedicationExist = checkIfTheMedicationExistInDb(medicationName);
        bool isAddSuccess = false;

        if (isMedicationExist)
        {
            // insert medication to the db
            /// if i secussees to insert the medication 
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