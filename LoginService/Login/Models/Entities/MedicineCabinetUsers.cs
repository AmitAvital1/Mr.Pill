using System.ComponentModel.DataAnnotations;

namespace Login.Models.LoginService;
public class MedicineCabinetUsers
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int MedicineCabinetId { get; set; }
    public required MedicineCabinet MedicineCabinet { get; set; }
}