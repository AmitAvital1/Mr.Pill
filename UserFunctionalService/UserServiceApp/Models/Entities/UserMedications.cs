namespace UserServiceApp.Models;
public class UserMedications
{
    public int Id { get; set; }
    public required string Barcode { get; set; }
    public int? PillSize { get; set; } 
    public DateTime Validity { get; set; }
    public int CreatorId { get; set; }
    public required User Creator { get; set; }
    public required MedicineCabinet MedicineCabinet { get; set; }
    public int MedicineCabinetId { get; set; }
    public bool IsPrivate  { get; set; }
    public int MedicationRepoId { get; set; }
    public  required MedicationRepo MedicationRepo { get; set; }
    public ICollection<Reminder>? Reminders { get; set; } 
    public int NumberOfPills { get; set; }
}

