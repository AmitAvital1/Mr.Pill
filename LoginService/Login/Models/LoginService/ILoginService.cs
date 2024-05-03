namespace Login.Models.LoginService;

public interface ILoginService
{
    public bool PhoneNumberExistInDb(int PhoneNumber);
    public string GenerateUserToken();
}