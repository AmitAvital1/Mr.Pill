public class ReminderDTO
{
    public int UserId { get; set; }
    public DateTime ReminderTime { get; set; }
    public string Message { get; set; }
    public bool IsRecurring { get; set; }
    public TimeSpan RecurrenceInterval { get; set; }
}