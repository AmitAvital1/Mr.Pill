using System.ComponentModel.DataAnnotations;

namespace Login.Models.LoginService;
public class UserHouse
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int HouseId { get; set; }
    public required House House { get; set; }
}