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
    public void SetReminder(ReminderDTO reminderDto, int phoneNumber)
    {
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if(user == null)
        {
            throw new Exception("No user found on phone number " + phoneNumber);
        }
        
        var medicationRepo = _dbContext?.MedicationRepos
                ?.FirstOrDefault(u => u.Id == reminderDto.UserMedicationId);
            
        if(medicationRepo == null)
        {
            throw new Exception("Invalid user medication id " + reminderDto.UserMedicationId);
        }

        var reminder = new Reminder
            {
                UserId = user.UserId,
                ReminderTime = reminderDto.ReminderTime,
                //Message = reminderDto.Message,
                IsRecurring = reminderDto.IsRecurring,
                RecurrenceInterval = reminderDto.RecurrenceInterval,
                IsActive = true,
                UserMedicationId = reminderDto.UserMedicationId

            };

            _dbContext.Reminders.Add(reminder);
            _dbContext.SaveChanges();

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
        _logger.LogInformation("Extracting phone number from token: {Token}", token);

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
        var phoneNumberClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "PhoneNumber");

        if (phoneNumberClaim == null || string.IsNullOrEmpty(phoneNumberClaim.Value))
        {
            _logger.LogWarning("Phone number claim not found or is empty in the token.");
            throw new InvalidOperationException("Phone number claim not found in token");
        }

        _logger.LogInformation("Extracted phone number: {PhoneNumber}", phoneNumberClaim.Value);
        return phoneNumberClaim?.Value!;
    }

    public bool IsUserExistInDb(int PhoneNumber)
    {
        if (_dbContext.Users != null)
        {
            return _dbContext.Users.Any(user => user.PhoneNumber == PhoneNumber);
        }

        return false;
    }

}
