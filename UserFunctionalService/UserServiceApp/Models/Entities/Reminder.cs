namespace UserServiceApp.Models;
public class Reminder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime ReminderTime { get; set; }
    public string? Message { get; set; }
    public bool IsRecurring { get; set; }
    public TimeSpan RecurrenceInterval { get; set; }
    public bool IsActive { get; set; }
    public int UserMedicationId { get; set; }
    public UserMedications? UserMedication { get; set; }
}