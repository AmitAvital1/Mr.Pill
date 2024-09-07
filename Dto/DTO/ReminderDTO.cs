public class ReminderDTO
{
    public DateTime ReminderTime { get; set; } // 14:30
    public string? Message { get; set; } // "לפני האוכל"
    public bool IsRecurring { get; set; } 
    public TimeSpan RecurrenceInterval { get; set; }
    public uint numOfPills { get; set; }
    public int UserMedicationId { get; set; }
}