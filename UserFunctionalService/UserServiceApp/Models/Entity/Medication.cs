namespace UserServiceApp.Models;

public class Medication
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime Validity { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
     public int MedicationRepoId { get; set; }
    public  required MedicationRepo MedicationRepo { get; set; }
}