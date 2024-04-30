namespace MrPill.DTOs.DTOs;

public class UserDTO
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int PhoneNumber { get; set; }
    public int HouseId { get; set; }

    public class UserDTOBuilder
    {
        private UserDTO _userDTO;

        public UserDTOBuilder()
        {
            _userDTO = new UserDTO();
        }

        public UserDTOBuilder WithUserId(int userId)
        {
            _userDTO.UserId = userId;
            return this;
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

        public UserDTOBuilder WithHouseId(int houseId)
        {
            _userDTO.HouseId = houseId;
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

