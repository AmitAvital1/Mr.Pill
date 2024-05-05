using MrPill.DTOs.DTOs;
using Login.Models.LoginService;

namespace Login.Models.MapperObject;

public class Mapper
{
    private static Mapper? instance;
    private static readonly object lockObject = new();

    private Mapper () {}
    
    public static Mapper Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new Mapper();
                    }
                }
            }
            
            return instance;
        }
    } 

    public UserDTO CreateAUserDtoFromUserObject(User user)
    {
        return UserDTO.Builder()
                .WithFirstName(user.FirstName)
                .WithLastName(user.LastName)
                .WithPhoneNumber(user.PhoneNumber)
                .Build();

    }
}