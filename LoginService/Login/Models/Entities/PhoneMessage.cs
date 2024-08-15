using System.ComponentModel.DataAnnotations;

namespace Login.Models.LoginService;
public class PhoneMessage
{
    [Key]
    public int Id { get; set; }
    public int PhoneNumber { get; set; }
    public DateTime SentTime { get; set; }
}