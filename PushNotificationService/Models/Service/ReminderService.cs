using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MrPill.DTOs.DTOs;
using Newtonsoft.Json;
using PN.Models.DB;
using System.IdentityModel.Tokens.Jwt;

namespace PN.Models.ReminderService;

public class ReminderService : IReminderService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger _logger;

       public ReminderService(AppDbContext dbContext, ILogger<ReminderService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

// TODO
    public void SetReminder(ReminderDTO reminderDto)
    {
        var reminder = new Reminder
            {
                UserId = reminderDto.UserId,
                ReminderTime = reminderDto.ReminderTime,
                Message = reminderDto.Message,
                IsRecurring = reminderDto.IsRecurring,
                RecurrenceInterval = reminderDto.RecurrenceInterval,
                IsActive = true
            };

            _dbContext.Reminders.Add(reminder);
            _dbContext.SaveChanges();

    }

}
