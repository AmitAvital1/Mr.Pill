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
    
    [AllowAnonymous]
    [HttpGet]
    [Route("Health")]
    public IActionResult Health()
    {
        return Ok("arrive!");
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
    public async Task<ActionResult> CreateNewMedication([FromBody] AddMedicationDto addMedicationDto)
    {
        try
        {
            string medicationBarcode = addMedicationDto.MedicationBarcode;
            bool privacy = addMedicationDto.Privacy;
            string? token = GetAuthorizationTokenOrThrow();
            int phoneNumber = _userService.GetPhoneNumberFromToken(token);

            _logger.LogInformation("Medication Barcode: {MedicationBarcode}", medicationBarcode);

            if (!_userService.IsUserExistInDb(phoneNumber))
            {
                _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (checked by userService)", phoneNumber);
                return NotFound(new { Message = "Phone number does not exist", PhoneNumber = phoneNumber });
            }

            bool success = await _userService.CreateNewMedication(medicationBarcode, phoneNumber, privacy);

            if (!success)
            {
                _logger.LogError("Failed to create a new medication for phone number {PhoneNumber} with barcode {MedicationBarcode}", phoneNumber, medicationBarcode);
                return StatusCode(500, "Failed to create a new medication. Please try again later.");
            }

            _logger.LogInformation("Successfully created a new medication with barcode {MedicationBarcode} for user {PhoneNumber}.", medicationBarcode, phoneNumber);

            return Ok(new { Message = "Medication created successfully.", PhoneNumber = phoneNumber, MedicationBarcode = medicationBarcode });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new medication.");
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet("user/medications")]
    public ActionResult<IEnumerable<MedicationDTO>> GetAllMedicationByUserId([FromQuery] PrivacyStatus privacyStatus)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumer = _userService.GetUserPhoneNumber(token);
            IEnumerable<MedicationDTO> medications = _userService.GetAllMedicationByUserId(userPhoneNumer, privacyStatus);

            return HandleMedicationResponse(medications, userPhoneNumer, privacyStatus);
        }
        catch (UnauthorizedAccessException ex)
        {
            return HandleUnauthorizedAccess(ex);
        }
        catch (Exception ex)
        {
           return HandleUnexpectedError(ex);
        }
    }

    [HttpGet("medication/barcode")]
    public async Task<ActionResult<MedicationDTO>> GetMedicationByBarcode([FromQuery] string medicationBarcode)
    {
        // we can support the functionality that give a medication by barcode without add it to the user 
        MedicationDTO medication = await _userService.GetMedicationByBarcode(medicationBarcode);
        return Ok(medication);
    }

    [HttpGet("notifications")]
    public ActionResult GetMyNotification()
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumer = _userService.GetUserPhoneNumber(token);

            if (_userService.IsManager(userPhoneNumer))
            {
                IEnumerable<UserDTO> userDTOs = _userService.GetAllUsersThatWantToBePartOfMyHome(userPhoneNumer);
                return Ok(userDTOs);
            }

            _logger.LogWarning("User with phone number {UserPhoneNumer} attempted to access manager-only notifications but is not a manager.", userPhoneNumer);
            return BadRequest(new { message = $"This user with phone {userPhoneNumer} is not a manager", userPhoneNumer });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving notifications for user. ");
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpPut("medications/{medicationId}")]
    public ActionResult UpdateMedication(int medicationId, [FromBody] MedicationDTO medicationDto)
    {
        // Update logics
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            
            _userService.UpdateMedication(medicationDto);
            _logger.LogInformation("Successfully updated medication with ID {MedicationId}.", medicationId);

            return Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating medication with ID {MedicationId}.", medicationId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        } 
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
            string token = GetAuthorizationTokenOrThrow();
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

    private ActionResult<IEnumerable<MedicationDTO>> HandleMedicationResponse(IEnumerable<MedicationDTO> medications, int userPhoneNumber, PrivacyStatus privacyStatus)
    {
        if (medications == null || !medications.Any())
        {
            return NotFound(new 
            { 
                Message = "No medications found for the user.", 
                UserPhoneNumber = userPhoneNumber, 
                PrivacyStatus = privacyStatus 
            });
        }

        return Ok(new 
        { 
            Message = "Medications retrieved successfully.", 
            UserPhoneNumber = userPhoneNumber, 
            PrivacyStatus = privacyStatus, 
            Medications = medications 
        });
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

    private ActionResult HandleUnauthorizedAccess(UnauthorizedAccessException ex)
    {
        _logger.LogError(ex, "Unauthorized access attempt while fetching medications.");
        return Unauthorized(new { Message = "Unauthorized access. Please provide a valid token." });
    }

    private ActionResult HandleUnexpectedError(Exception ex)
    {
        _logger.LogError(ex, "An error occurred while fetching medications for user.");
        return StatusCode(500, "An unexpected error occurred. Please try again later.");
    }

    private string GetAuthorizationToken()
    {
        return HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last() !;
    }

    private int GetCurrentPort()
    {
        int serverPort = HttpContext.Connection.LocalPort;
        return serverPort;
    }
}