using MrPill.DTOs.DTOs;

namespace PN.Models.ReminderService;

public interface IReminderService
{
    public void SetReminder(ReminderDTO reminderDto, int phoneNumber);
    public bool IsUserExistInDb(int PhoneNumber);
    public int GetPhoneNumberFromToken(string? token);
    public IEnumerable<UIReminderDTO> GetUserReminders(int phoneNumber);
    public IEnumerable<UIReminderDTO> GetUserTodayReminders(int phoneNumber);
    void EditReminder(ReminderDTO reminderDto, int phoneNumber, int Id);
    void DeleteReminder(int phoneNumber, int Id);
    void ApproveReminder(int phoneNumber, int Id, bool approved);

}