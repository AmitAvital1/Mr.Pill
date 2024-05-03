using System.ComponentModel.DataAnnotations;

namespace MrPill.DTOs.DTOs;

public class UserDTO
{
    [Required(ErrorMessage = "Please enter a name.")]
    [StringLength(50, ErrorMessage = "First Name should not exceed 50 characters.")]
    public string? FirstName { get; set; }
    
    [Required(ErrorMessage = "Please enter a name.")]
    [StringLength(50, ErrorMessage = "Last Name should not exceed 50 characters.")]
    public string? LastName { get; set; }
    
    [Required(ErrorMessage = "Please enter a phone number.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    public int PhoneNumber { get; set; }
    
    public class UserDTOBuilder
    {
        private UserDTO _userDTO;

        public UserDTOBuilder()
        {
            _userDTO = new UserDTO();
        }

        public UserDTOBuilder WithFirstName(string firstName)
        {
            _userDTO.FirstName = firstName;
            return this;
        }

        public UserDTOBuilder WithLastName(string lastName)
        {
            _userDTO.LastName = lastName;
            return this;
        }

        public UserDTOBuilder WithPhoneNumber(int phoneNumber)
        {
            _userDTO.PhoneNumber = phoneNumber;
            return this;
        }

        public UserDTO Build()
        {
            return _userDTO;
        }
    }

    public static UserDTOBuilder Builder()
    {
        return new UserDTOBuilder();
    }
}

