
namespace UserServiceApp.Models;
public class User
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public int HouseId { get; set; }
    public House? House { get; set; }
    public ICollection<UserHouse>? UserHouses { get; set; }
    public ICollection<Medication>? Medications { get; set; }
}