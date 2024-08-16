namespace MrPill.DTOs.DTOs;

using System.ComponentModel.DataAnnotations;

public class PhoneNumberDTO
{
    [Required(ErrorMessage = "Phone number is required")]
    public string? PhoneNumber { get; set; }
}