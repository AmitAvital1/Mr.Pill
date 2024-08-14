
using System.ComponentModel.DataAnnotations;

namespace UserServiceApp.Models;
public class MedicineCabinetUsers
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int MedicineCabinetId { get; set; }
    public required MedicineCabinet MedicineCabinet { get; set; }
}