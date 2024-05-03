namespace UserServiceApp.Models;

public class UserMedications
{
    public int Id { get; set; }
    public required string Barcode { get; set; }
    public int? PillSize { get; set; } 
    public DateTime Validity { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
     public int MedicationRepoId { get; set; }
    public  required MedicationRepo MedicationRepo { get; set; }
}