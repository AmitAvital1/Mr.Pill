using Microsoft.AspNetCore.Mvc;
namespace PN.Controllers;
using MrPill.DTOs.DTOs;
using Microsoft.AspNetCore.Authorization;
using PN.Models.ReminderService;
using PN.Models;

[Authorize]
public class ReminderController : Controller
{
    private readonly ILogger<ReminderController> _logger;
    private readonly IReminderService _reminderService;

     public ReminderController(ILogger<ReminderController> logger, IReminderService reminderService)
    {
        _logger = logger;
        _reminderService = reminderService;
    }

    [HttpGet]
    [Route("Reminders")]
    public ActionResult<IEnumerable<ReminderDTO>> GetReminders()
    {
        string? token = GetAuthorizationTokenOrThrow();
        int phoneNumber = _reminderService.GetPhoneNumberFromToken(token);

        if (!_reminderService.IsUserExistInDb(phoneNumber))
        {
            _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (checked by userService)", phoneNumber);
            return NotFound(new { Message = "Phone number does not exist", PhoneNumber = phoneNumber });
        }

        IEnumerable<UIReminderDTO> reminders = _reminderService.GetUserReminders(phoneNumber);

        return Ok(reminders);
    }

    [HttpPost]
    [Route("SetReminder")]
    public IActionResult SetReminder([FromBody] ReminderDTO reminderDto)
    {
        try
        {
            string? token = GetAuthorizationTokenOrThrow();
            int phoneNumber = _reminderService.GetPhoneNumberFromToken(token);

            if (!_reminderService.IsUserExistInDb(phoneNumber))
            {
                _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database (checked by userService)", phoneNumber);
                return NotFound(new { Message = "Phone number does not exist", PhoneNumber = phoneNumber });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state for reminder" });
            }

            _reminderService.SetReminder(reminderDto, phoneNumber);
            
            return Ok(new { Message = "Reminder set successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while setting reminder");
            return StatusCode(500, "Internal Server Error " + ex.Message);
        }
    }

    [HttpPost]
    [Route("EditReminder")]
    public IActionResult EditReminder([FromBody] ReminderDTO reminderDto)
    {
        return null;
    }
    
    [HttpPost]
    [Route("DeleteReminder")]
    public IActionResult DeleteReminder(int reminderId)
    {
        return null;
    }

     private string GetAuthorizationTokenOrThrow()
    {
        string token = GetAuthorizationToken() ?? throw new Exception("Authorization token not provided");
        return token;
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
}

