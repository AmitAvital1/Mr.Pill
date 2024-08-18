using MrPill.DTOs.DTOs;

namespace PN.Models.ReminderService;

public interface IReminderService
{
    public void SetReminder(ReminderDTO reminderDto);
}