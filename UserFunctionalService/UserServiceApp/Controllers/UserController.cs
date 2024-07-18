using Microsoft.AspNetCore.Mvc;
namespace UserServiceApp.Controllers;
using UserServiceApp.Models.UserService;
using MrPill.DTOs.DTOs;
using Microsoft.AspNetCore.Authorization;
using UserServiceApp.Models;
using UserServiceApp.Models.ManagerService;
using UserServiceApp.Models.Exceptions;

[Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IManagerService _managerService;

    public UserController(ILogger<UserController> logger, IUserService userService, IManagerService managerService)
    {
        _logger = logger;
        _userService = userService;
        _managerService = managerService;
        
        // the _managerService charge on the job how run in the background
        // _managerService.StartAsync();

        // Task task = Task.Run(() =>
        // {
            // SendRequestToGateway();
        // });
    }
    
    private int GetCurrentPort()
    {
        int serverPort = HttpContext.Connection.LocalPort;
        return serverPort;
    }

    private void SendRequestToGateway()
    {
        try
        {
            int port = GetCurrentPort();
           
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api-gateway-url.com/endpoint?port={port}")
            };

            using (HttpClient client = new())
            {
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Request sent to API gateway successfully");
                }
                else
                {
                    _logger.LogError($"Failed to send request to API gateway. Status code: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while sending request to API gateway: {ex.Message}");
        }
    }

    [HttpPost("medications")]
    public async Task<ActionResult> CreateNewMedication([FromBody] string medicationBarcode, bool privatcy)
    {
        string? token = GetAuthorizationToken();
        int phoneNumber = _userService.GetPhoneNumberFromToken(token);

        if (!_userService.IsUserExistInDb(phoneNumber))
        {
            _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (this check was made by userService)", phoneNumber);
            return NotFound("Phone number does not exist");
        }

        bool success = await _userService.CreateNewMedication(medicationBarcode, phoneNumber, privatcy);
        return success ? Ok() : StatusCode(500);
    }

    [HttpGet("user/medications")]
    public ActionResult<IEnumerable<MedicationDTO>> GetAllMedicationByUserId([FromQuery] PrivacyStatus  privacyStatus)
    {
        string? token = GetAuthorizationTokenOrThrow();
        int userPhoneNumer = _userService.GetUserPhoneNumber(token);
        IEnumerable<MedicationDTO> medications = _userService.GetAllMedicationByUserId(userPhoneNumer, privacyStatus);
        
        return Ok(medications);
    }

    [HttpGet("medication/barcode")]
    public async Task<ActionResult<MedicationDTO>> GetMedicationByBarcode([FromQuery] string medicationBarcode)
    {
        // need to authorize the endpoint and check what is the id of the user that send the request
        MedicationDTO medication = await _userService.GetMedicationByBarcode(medicationBarcode);
        return Ok(medication);
    }

    [HttpGet("notifications")]
    public ActionResult GetMyNotification()
    {
        string? token = GetAuthorizationTokenOrThrow();
        int userPhoneNumer = _userService.GetUserPhoneNumber(token);

        if (_userService.IsManager(userPhoneNumer))
        {
            IEnumerable<UserDTO> userDTOs = _userService.GetAllUsersThatWantToBePartOfMyHome(userPhoneNumer);
            return Ok(userDTOs);
        }

        return BadRequest(new { message = @"This User With Phone {userPhoneNumer} is not a manager",userPhoneNumer });
    }

    [HttpPut("medications/{medicationId}")]
    public ActionResult UpdateMedication(int medicationId, [FromBody] MedicationDTO medicationDto)
    {
        // Update logic
       string? token = GetAuthorizationToken();

        if (token == null)
        {
            return BadRequest(new { message = "Authorization token not provided" });
        }

        _userService.UpdateMedication(medicationDto);
       
        return Ok();
    }

    [HttpDelete("medications/{medicationId}")]
    public ActionResult DeleteMedication(int medicationId)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumber = _userService.GetUserPhoneNumber(token);

            _userService.DeleteMedication(userPhoneNumber, medicationId);

            return Ok();
        }
        catch (Exception ex)
        {
           
            _logger.LogError(ex, $"An error occurred at {GetCurrentFormattedTime()} while deleting medication");
            
            return StatusCode(500, new { message = "An error occurred while deleting medication" });
        }
    }

    [HttpPost("inviteToMyHouse/{phoneNumber}")]
    public ActionResult InviteNewMemberToJoindMyHouse(int phoneNumber)
    {
        try
        {
            string token = GetAuthorizationToken();
            int managerPhoneNumber = _userService.GetUserPhoneNumber(token!);

            if (_userService.IsManager(managerPhoneNumber))
            {
                _userService.InviteMemberToJoindMyHouse(managerPhoneNumber, phoneNumber);
                return Ok();
            }
            else
            {
                throw new NotAuthorizedException("User is not authorized to add a member to the house.");
            }
        
        }
        catch (NotAuthorizedException ex)
        {
            _logger.LogError(ex, $"User is not authorized at {GetCurrentFormattedTime()} to add a member to the house.");

            return StatusCode(403, new { message = "User is not authorized to perform this action." });
        }
        catch (Exception ex)
        {
             string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogError(ex, $"An error occurred at {currentTime} while try to add a member with phone number {phoneNumber} to my house.");
            
            return StatusCode(500, new { message = "An error occurred while try to add a new member to my house." });
        }
    }

    private string GetAuthorizationTokenOrThrow()
    {
        string token = GetAuthorizationToken() ?? throw new Exception("Authorization token not provided");
        return token;
    }

    private string GetCurrentFormattedTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private string GetAuthorizationToken()
    {
        return HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last() !;
    }
}