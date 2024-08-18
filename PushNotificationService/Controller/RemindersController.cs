using Microsoft.AspNetCore.Mvc;
namespace PN.Controllers;
using MrPill.DTOs.DTOs;
using Microsoft.AspNetCore.Authorization;
using PN.Models.ReminderService;

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

    [HttpPost]
    [Route("SetReminder")]
    public IActionResult SetReminder([FromBody] ReminderDTO reminderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state for reminder" });
            }
            
            return Ok(new { Message = "Reminder set successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while setting reminder.");
            return StatusCode(500, "Internal Server Error");
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
}