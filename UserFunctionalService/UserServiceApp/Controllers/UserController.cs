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
        
        initFunction();
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("Health")]
    public IActionResult Health()
    {
        return Ok("arrive to user controller!");
    }


/////////////////// -> POST ///////////////////


    [HttpPost("medicine-cabinet")]
    public ActionResult CreateNewMedicineCabinet([FromQuery] string Name)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int phoneNumber = _userService.GetPhoneNumberFromToken(token);

            if (!_userService.IsUserExistInDb(phoneNumber))
            {
                _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (checked by userService)", phoneNumber);
                return NotFound(new { Message = "Phone number does not exist", PhoneNumber = phoneNumber });
            }

            if (_userService.NameAlreadyExistInMyInventory(Name, phoneNumber))
            {
               _logger.LogWarning(
                    "Attempt to add a medicine cabinet with an already existing name '{Name}' for user with phone number {PhoneNumber}.", 
                    Name, 
                    phoneNumber
                ); 
                
                return BadRequest(new { Message = $"The name '{Name}' is already taken in your inventory." });
            }

            _userService.CreateNewMedicineCabinet(Name, phoneNumber);

            return Ok(new { Message = "Medicine cabinet created successfully", MedicineCabinetName = Name });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new medicine cabinet with name '{Name}' for user.", Name);
            return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpPost("medications")]
    public async Task<ActionResult> CreateNewMedication([FromBody] AddMedicationDto addMedicationDto,[FromQuery] string medicineCabinetName)
    {
        try
        {
            if (medicineCabinetName == null)
            {
                return StatusCode(405, "Invalid medicine cabinet name");
            }
            
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

            bool success = await _userService.CreateNewMedication(medicationBarcode, phoneNumber, privacy, medicineCabinetName);

            if (!success)
            {
                _logger.LogError(
                    "Failed to create a new medication for phone number {PhoneNumber} with barcode {MedicationBarcode}", 
                    phoneNumber, 
                    medicationBarcode
                );

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

/////////////////// -> GET ///////////////////


    [HttpGet("user/medications")]
    public ActionResult<IEnumerable<MedicationDTO>> GetAllMedicationByUserId([FromQuery] string medicineCabinetName)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumer = _userService.GetUserPhoneNumber(token);
            IEnumerable<MedicationDTO> medications = _userService.GetAllMedicationByUserId(userPhoneNumer, medicineCabinetName);

            return HandleMedicationResponse(medications, userPhoneNumer);
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

    [HttpGet("user/all/medications")]
    public ActionResult<IEnumerable<MedicationDTO>> GetAllMedication()
    {
        // the recomended endpoint
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumer = _userService.GetUserPhoneNumber(token);
            IEnumerable<MedicationDTO> medications = _userService.GetAllMedication(userPhoneNumer);

            return HandleMedicationResponse(medications, userPhoneNumer);
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

    [HttpGet("user/cabinet")]
    public ActionResult GetAllCabinets()
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumer = _userService.GetUserPhoneNumber(token);
            IEnumerable<MedicineCabinetDTO> medicineCabinetDTOs = _userService.GetAllMedicineCabinets(userPhoneNumer);

            if (!medicineCabinetDTOs.Any())
            {
                return NotFound(new { Message = "No medicine cabinets found for the user.", UserPhoneNumber = userPhoneNumer });
            }

            return Ok(medicineCabinetDTOs);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving medicine cabinets for user.");
            return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpGet("users/cabinet")]
    public ActionResult GetAllCabinetUsers([FromQuery] int cabinetId)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumer = _userService.GetUserPhoneNumber(token);
            IEnumerable<UserDTO> medicineCabinetDTOs = _userService.GetAllMedicineCabinetUsers(userPhoneNumer, cabinetId);

            if (!medicineCabinetDTOs.Any())
            {
                return NotFound(new { Message = "No users how member is this cabinet id found for the user.", UserPhoneNumber = userPhoneNumer });
            }

            return Ok(medicineCabinetDTOs);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving members cabinet for this user.");
            return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpGet("medication/barcode")]
    public async Task<ActionResult<MedicationDTO>> GetMedicationByBarcode([FromQuery] string medicationBarcode)
    {
        // we can support the functionality that give a medication by barcode without add it to the user 
        MedicationDTO medication = await _userService.GetMedicationByBarcode(medicationBarcode);
        return Ok(medication);
    }



/////////////////// -> PUT ///////////////////


    [HttpPut("medications/update")]
    public ActionResult UpdateMedication([FromBody] UpdateMedicationDTO updateMedication)
    {
        // This endpoint update the number of pills in the medications
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            _userService.UpdateMedicationPills(updateMedication);
            _logger.LogInformation("Successfully updated medication (Pills) with ID {MedicationId}.", updateMedication.MedicationId);

            var response = new 
            {
                Message = "Medication updated successfully.",
                MedicationId = updateMedication.MedicationId,
                Amount = updateMedication.Amount,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating medication with ID {MedicationId}.", updateMedication.MedicationId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        } 
    }


    [HttpPut("update/medication/date")]
    public ActionResult UpdateExistingMedication([FromBody] UpdateDateMedicationDTO updateDateMedication)
    {
        // This endpoint update the expire date of medications
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            _userService.UpdateDateMedication(updateDateMedication);
            _logger.LogInformation("Successfully updated medication with ID {MedicationId}.", updateDateMedication.MedicationId);

            var response = new 
            {
                Message = "Medication updated successfully.",
                MedicationId = updateDateMedication.MedicationId,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating medication with ID {MedicationId}.", updateDateMedication.MedicationId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        } 
    }



/////////////////// -> DELETE ///////////////////

    
    
    [HttpDelete("medications/{medicationId}")]
    public ActionResult DeleteMedication(int medicationId, [FromQuery] string medicineCabinetName)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumber = _userService.GetUserPhoneNumber(token);
            _userService.DeleteMedication(userPhoneNumber, medicationId, medicineCabinetName);

            _logger.LogInformation(
                "Medication with ID {MedicationId} successfully deleted for user {UserPhoneNumber} from cabinet '{MedicineCabinetName}'", 
                medicationId, 
                userPhoneNumber, 
                medicineCabinetName
            );

            return Ok(new 
            { 
                message = $"Medication with ID {medicationId} successfully deleted from cabinet '{medicineCabinetName}' for user {userPhoneNumber}.",
                medicationId,
                userPhoneNumber,
                medicineCabinetName
            });
        }
        catch (MedicationDeletionException ex)
        {
            _logger.LogWarning(ex, $"Conflict while deleting medication with ID {medicationId}");
            // Return 409 Conflict for active reminders or alerts preventing deletion
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Invalid operation while deleting medication with ID {medicationId}");
            // Return 404 if medication is not found
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred at {GetCurrentFormattedTime()} while deleting medication");
            return StatusCode(500, new { message = "An error occurred while deleting medication" });
        }
    }

    [HttpDelete("cabinet/user/remove-member")]
    public ActionResult RemoveMemberFromHouse([FromQuery] int targetToRemovePhoneNumber, [FromQuery] int cabinetId)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int userPhoneNumber = _userService.GetUserPhoneNumber(token);

            _userService.RemoveMemberFromHouse(userPhoneNumber,targetToRemovePhoneNumber, cabinetId);

            _logger.LogInformation($"- Successfully removed PhoneNumber {targetToRemovePhoneNumber} from CabinetId {cabinetId}.");
            return Ok(new { message = $"Member with PhoneNumber {targetToRemovePhoneNumber} successfully removed from Cabinet {cabinetId}." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred at {GetCurrentFormattedTime()} while deleting member");
            return StatusCode(500, new { message = "An error occurred while deleting members." });
        }
    }


/////////////////// -> PRIVATE METHODS ///////////////////



    private ActionResult<IEnumerable<MedicationDTO>> HandleMedicationResponse(IEnumerable<MedicationDTO> medications, int userPhoneNumber)
    {
        if (medications == null || !medications.Any())
        {
            return NotFound(new 
            { 
                Message = "No medications found for the user.", 
                UserPhoneNumber = userPhoneNumber, 
            });
        }

        return Ok(new 
        { 
            Message = "Medications retrieved successfully.", 
            UserPhoneNumber = userPhoneNumber, 
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

    private void initFunction()
    {
        // the _managerService charge on the job how run in the background
        // _managerService.StartAsync();

        // Task task = Task.Run(() =>
        // {
            // SendRequestToGateway();
        // });
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
}