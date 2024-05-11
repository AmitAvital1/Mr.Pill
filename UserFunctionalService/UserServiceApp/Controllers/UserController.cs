using Microsoft.AspNetCore.Mvc;
namespace UserServiceApp.Controllers;
using UserServiceApp.Models.UserService;
using MrPill.DTOs.DTOs;
using Microsoft.AspNetCore.Authorization;
using UserServiceApp.Models;

[Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;

        // Task task = Task.Run(() =>
        // {
        //     sendRequestToGateway();
        // });
    }

    private void sendRequestToGateway()
    {
        try
        {
            int port = HttpContext.Connection.LocalPort;

            // if i have some instances of the server i need to suffly the name of the server
           
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
        string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
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
        string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token == null)
        {
            return BadRequest(new { message = "Authorization token not provided" });
        }

        int userPhoneNumer = _userService.GetUserPhoneNumber(token);
        
        IEnumerable<MedicationDTO> medications = _userService.GetAllMedicationByUserId(userPhoneNumer, privacyStatus);
        return Ok(medications);
    }

    [HttpGet("medications")]
    public ActionResult<MedicationDTO> GetMedicationByName([FromQuery] string medicationName)
    {
        // need to authorize the endpoint and check what is the id of the user that send the request
        MedicationDTO medication = _userService.GetMedicationByName(medicationName);
        return Ok(medication);
    }

    [HttpPut("medications/{medicationId}")]
    public ActionResult UpdateMedication(int medicationId, [FromBody] MedicationDTO medicationDto)
    {
        // Update logic
        return Ok();
    }

    [HttpDelete("medications/{medicationId}")]
    public ActionResult DeleteMedication(int medicationId)
    {
        // Delete logic
        return Ok();
    }
}