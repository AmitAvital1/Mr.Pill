public class ReminderDTO
{
    public DateTime ReminderTime { get; set; }
    public string? Message { get; set; }
    public bool IsRecurring { get; set; }
    public TimeSpan RecurrenceInterval { get; set; }
    public int UserMedicationId { get; set; }
}