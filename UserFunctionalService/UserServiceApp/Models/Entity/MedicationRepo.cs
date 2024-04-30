namespace UserServiceApp.Models;
public class MedicationRepo
{
    public int Id { get; set; }
    public ICollection<Medication>? Medications { get; set; }
}