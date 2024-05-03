namespace UserServiceApp.Models;

public class Medication
{
    public int Id { get; set; }
    public required string DrugEnglishName { get; set; }
    public required string DrugHebrewName { get; set; }
    public string? EnglishDescription { get; set; }
    public string? HebrewDescription { get; set; }
    public required string Barcode { get; set; }
    public string? ImagePath { get; set; }
    public int? PillSize { get; set; } 
    public DateTime Validity { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
     public int MedicationRepoId { get; set; }
    public  required MedicationRepo MedicationRepo { get; set; }
}