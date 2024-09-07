using System.ComponentModel.DataAnnotations;

namespace PN.Models;
public class UserMedications
{
    [Key]
    public int Id { get; set; }
    public required string Barcode { get; set; }
    public int? PillSize { get; set; } 
    public DateTime? Validity { get; set; }
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


// need to add validity and duration
// boolean that check if the user takes it daily 
// the number of pills in the pill
// how many times a day the pill been taken