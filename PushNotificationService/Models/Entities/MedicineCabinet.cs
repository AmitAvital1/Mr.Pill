namespace PN.Models;
public class MedicineCabinet
{
    public int Id { get; set; }
    public required string MedicineCabinetName { get; set; }
    public User? Creator { get; set; }
     public int? CreatorId { get; set; }
    public ICollection<MedicineCabinetUsers>? MedicineCabinetUsers { get; set; }
    public ICollection<UserMedications>? Medications { get; set; }
}