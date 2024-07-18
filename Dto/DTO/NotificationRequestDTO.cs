namespace MrPill.DTOs.DTOs;


public class NotificationRequestDTO 
{
    public required string Token { get; set; }
    public required string Title { get; set; }
    public string ?Body { get; set; }
}