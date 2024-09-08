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

    public void SetReminder(ReminderDTO reminderDto, int phoneNumber)
    {
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            throw new Exception("No user found on phone number " + phoneNumber);
        }
        
        var medicationRepoId = _dbContext?.UserMedications
                ?.FirstOrDefault(u => u.Id == reminderDto.UserMedicationId).MedicationRepoId;
        
        var medicationRepo = _dbContext?.MedicationRepos
                ?.FirstOrDefault(u => u.Id == medicationRepoId);
            
        if (medicationRepo == null)
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
                Approved = false,
                numOfPills = reminderDto.numOfPills,
                UserMedicationId = reminderDto.UserMedicationId

            };

            _dbContext.Reminders.Add(reminder);
            _dbContext.SaveChanges();
    }

    public void EditReminder(ReminderDTO reminderDto, int phoneNumber, int Id)
    {
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            throw new Exception("No user found on phone number " + phoneNumber);
        }
        
        var Reminder = _dbContext.Reminders
                .Where(r => r.Id == Id)
                .FirstOrDefault();
            
        if (Reminder == null)
        {
            throw new Exception("Invalid reminder id " + Id);
        }

        if (Reminder.UserId != user.UserId)
        {
            throw new Exception("Reminder not belong this user");
        }

        var reminder = new Reminder
            {
                UserId = user.UserId,
                ReminderTime = reminderDto.ReminderTime,
                //Message = reminderDto.Message,
                IsRecurring = reminderDto.IsRecurring,
                RecurrenceInterval = reminderDto.RecurrenceInterval,
                IsActive = true,
                numOfPills = reminderDto.numOfPills,
                UserMedicationId = reminderDto.UserMedicationId

            };

            Reminder.ReminderTime = reminderDto.ReminderTime;
            Reminder.RecurrenceInterval = reminderDto.RecurrenceInterval;
            Reminder.Message = reminderDto.Message;
            
            _dbContext.SaveChanges();
    }

    public void DeleteReminder(int phoneNumber, int Id)
    {
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            throw new Exception("No user found on phone number " + phoneNumber);
        }
        
        var Reminder = _dbContext.Reminders
                .Where(r => r.Id == Id)
                .FirstOrDefault();
            
        if (Reminder == null)
        {
            throw new Exception("Invalid reminder id " + Id);
        }

        if (Reminder.UserId != user.UserId)
        {
            throw new Exception("Reminder not belong this user");
        }

        _dbContext.Reminders.Remove(Reminder);
        _dbContext.SaveChanges();
    }

    public void ApproveReminder(int phoneNumber, int Id, bool approved)
    {
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            throw new Exception("No user found on phone number " + phoneNumber);
        }
        
        var Reminder = _dbContext.Reminders
                .Where(r => r.Id == Id)
                .FirstOrDefault();

        var medication = _dbContext?.UserMedications
                ?.FirstOrDefault(u => u.Id == Reminder.UserMedicationId);
            
        if (Reminder == null)
        {
            throw new Exception("Invalid reminder id " + Id);
        }

        if (Reminder.Approved)
        {
            throw new Exception("Reminder already approved by user");
        }

        if (Reminder.UserId != user.UserId)
        {
            throw new Exception("Reminder not belong this user");
        }
        if (!approved)
        {
            Reminder.Approved = true;
            _logger.LogInformation("User " + phoneNumber + " decline his reminder id " + Reminder.Id);
        }
        else
        {
            if (medication.NumberOfPills >= Reminder.numOfPills)
            {
                int n = (int)Reminder.numOfPills;
                medication.NumberOfPills = medication.NumberOfPills - n;
            }
            else
            {
                _logger.LogWarning("User " + phoneNumber + " has not enogh medication. Set to 0");
                medication.NumberOfPills = 0;
            }

            Reminder.Approved = true;
            _logger.LogInformation("User " + phoneNumber + " approved his reminder id " + Reminder.Id);
        }

        _dbContext.SaveChanges();
    }

    public IEnumerable<UIReminderDTO> GetUserReminders(int phoneNumber)
    {
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if(user == null)
        {
            throw new Exception("No user found on phone number " + phoneNumber);
        }
        
        var res = new List<UIReminderDTO>();

        var Reminders = _dbContext.Reminders
                .Where(r => r.UserId == user.UserId && r.IsActive)
                .ToList();
            
        foreach (var _reminder in Reminders)
        {
            UIReminderDTO dto = new UIReminderDTO();
            dto.ReminderId = _reminder.Id;
            dto.ReminderTime = _reminder.ReminderTime;
            dto.RecurrenceInterval = _reminder.RecurrenceInterval;
            dto.Approved = _reminder.Approved;

            var userMedication = _dbContext.UserMedications
                    .Where(um => um.Id == _reminder.UserMedicationId)
                    .FirstOrDefault();

            var medicationRepo = _dbContext.MedicationRepos
                    .Where(um => um.Id == userMedication.MedicationRepoId)
                    .FirstOrDefault();

            string cabinetName = _dbContext.MedicineCabinets
                    .Where(mc => mc.Id == userMedication.MedicineCabinetId)
                    .FirstOrDefault().MedicineCabinetName;

            dto.DrugHebrewName = medicationRepo.DrugHebrewName;
            dto.ImagePath = medicationRepo.ImagePath;
            dto.MedicineCabinetName = cabinetName;

            res.Add(dto);
        }

       return res;
    }

    public IEnumerable<UIReminderDTO> GetUserTodayReminders(int phoneNumber)
    {
        var reminders = GetUserReminders(phoneNumber);
        var res = new List<UIReminderDTO>();
        var now = DateTime.UtcNow.AddHours(3);

        foreach (var r in reminders)
        {
            if(r.ReminderTime.Date.Equals(now.Date))
            {
                if(!r.Approved ?? false)
                {
                    res.Add(r);
                }
            }
        }

       return res;
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
