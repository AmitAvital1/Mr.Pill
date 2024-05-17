using System.ComponentModel.DataAnnotations;

namespace UserServiceApp.Models;
public class UserHouse
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int HouseId { get; set; }
    public required House House { get; set; }
}