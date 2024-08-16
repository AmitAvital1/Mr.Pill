namespace MrPill.DTOs.DTOs;

using System.ComponentModel.DataAnnotations;

public class CodeValidationDTO
{
    [Required(ErrorMessage = "Phone number is required")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Verification code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits long")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be a 6-digit number")]
    public string? Code { get; set; }
}