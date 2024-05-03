namespace UserServiceApp.Models;
public class House
{
    public int Id { get; set; }
    public required string FamilyName { get; set; }
    public User? Manager { get; set; }
    public ICollection<UserHouse>? UserHouses { get; set; }
}