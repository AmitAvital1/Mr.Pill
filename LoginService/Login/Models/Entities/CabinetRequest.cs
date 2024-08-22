namespace Login.Models.LoginService;
public class CabinetRequest
{
    public int Id { get; set; }
    public string? TargetPhoneNumber { get; set; }
    public required string SenderPhoneNumber { get; set; }
    public required string CabinetName { get; set; }
    public bool IsHandle { get; set; }
    public bool IsApprove { get; set; }
    public bool IsSenderSeen { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
}