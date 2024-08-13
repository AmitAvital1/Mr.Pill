namespace MrPill.DTOs.DTOs;
public class AddMedicationDto
{
    public required string MedicationBarcode { get; set; }
    public bool Privacy { get; set; }
}