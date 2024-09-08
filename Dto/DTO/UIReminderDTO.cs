public class UIReminderDTO
{
    public int ReminderId { get; set; }
    public DateTime ReminderTime { get; set; }
    public TimeSpan RecurrenceInterval { get; set; }
    public string? DrugHebrewName { get; set; }
    public string? ImagePath { get; set; }
    public bool? Approved { get; set; }
    public string? MedicineCabinetName { get; set; }
}