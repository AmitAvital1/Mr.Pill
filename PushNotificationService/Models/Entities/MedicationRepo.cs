namespace PN.Models;
public class MedicationRepo
{
    public int Id { get; set; }
    public required string Barcode { get; set; }
    public required string DrugEnglishName { get; set; }
    public required string DrugHebrewName { get; set; }
    public string? EnglishDescription { get; set; }
    public string? HebrewDescription { get; set; }
    public string? ImagePath { get; set; }
    public ICollection<UserMedications>? Medications { get; set; } 
    
}