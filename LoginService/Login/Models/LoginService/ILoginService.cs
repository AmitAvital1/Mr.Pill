namespace Login.Models.LoginService;
using MrPill.DTOs.DTOs;

public interface ILoginService
{
    public bool PhoneNumberExistInDb(int PhoneNumber);
    public string GenerateUserToken(string phoneNumber);
    public bool RegisterUser(UserDTO userDTORegister);
    public Task<bool> AddNewHouseSuccsesfully(string token, int targetPhoneNumber, string medicineCabinetName);
    public string GenerateVerificationCode();
    public bool IsSameUser(int targetPhoneNumber, string phoneNumber);
    public void SaveVerificationCode(int phoneNumber, string? code);
    public bool ValidateVerificationCode(int phoneNumber, string? code);
    public IEnumerable<CabinetRequestDTO> GetAllRequestByUserToken(string token);
    public void HandleNotification(string token, int requestId, bool approve);
    public User GetUserByPhoneNumber(int phoneNumber);
    public string getPhoneNumberFromToken(string token);
}