namespace UserServiceApp.Models;
public class UserHouse
{
    public int UserId { get; set; }
    public required User User { get; set; }

    public int HouseId { get; set; }
    public required House House { get; set; }
}