namespace Login.Models.LoginService;
public class User
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public int MedicineCabinetsId { get; set; }
    public MedicineCabinets? MedicineCabinets { get; set; }
    public ICollection<MedicineCabinetUsers>? MedicineCabinetUsers { get; set; }
}