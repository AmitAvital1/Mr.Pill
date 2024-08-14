using System.ComponentModel.DataAnnotations;

namespace Login.Models.LoginService;
public class MedicineCabinetUsers
{
    [Key]
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public required User Creator { get; set; }
    public int MedicineCabinetsId { get; set; }
    public required MedicineCabinets MedicineCabinets { get; set; }
}