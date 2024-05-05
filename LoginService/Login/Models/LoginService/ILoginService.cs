namespace Login.Models.LoginService;
using MrPill.DTOs.DTOs;

public interface ILoginService
{
    public bool PhoneNumberExistInDb(int PhoneNumber);
    public string GenerateUserToken(string phoneNumber);
    public bool RegisterUser(UserDTO userDTORegister);
    public Task<bool> AddNewHouseSuccsesfully(string token, bool mergeToNewHouse, int managerPhone);
}