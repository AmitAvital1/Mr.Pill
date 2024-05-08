namespace UserServiceApp.Models;

public class HouseRequest
{
    public int Id { get; set; }
    public int HouseId { get; set; }
    public required string SenderPhoneNumber { get; set; }
    public bool IsHandle { get; set; }
    public bool IsApprove { get; set; }
    public bool IsSenderSeen { get; set; }
    public bool MergeToNewHouse { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
}