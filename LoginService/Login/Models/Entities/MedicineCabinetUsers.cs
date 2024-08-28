using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login.Models.LoginService;
public class MedicineCabinetUsers
{
    [Key]
    public int? Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int MedicineCabinetId { get; set; }
    public required MedicineCabinet MedicineCabinet { get; set; }
}

/// need to check if the nullable of the key is correct !!!!!!!!!!!!!!!!!!!!!!!!