using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Login.Models.DB;
using Microsoft.IdentityModel.Tokens;
using MrPill.DTOs.DTOs;
using Login.Models.MapperObject;
using Login.Models.RabbitMq;

namespace Login.Models.LoginService;

public class LoginService : ILoginService
{
    private readonly ILogger<LoginService> _logger;
    private readonly IConfiguration _config;
    private readonly AppDbContext _dbContext;
    private static readonly object _lockObject = new();

    public LoginService(ILogger<LoginService> logger,  IConfiguration configuration, AppDbContext appDbContext)
    {
        _logger = logger;
        _config = configuration;
        _dbContext = appDbContext;
    }

    public bool RegisterUser(UserDTO userDTORegister)
    {
        lock (_lockObject)
        {
            try
            {
                string phoneNumberString = userDTORegister.PhoneNumber !;
                int phoneNumberValue = int.Parse(phoneNumberString);

                if (!PhoneNumberExistInDb(phoneNumberValue))
                {
                    var newHouse = new House
                    {
                        FamilyName = userDTORegister.LastName !,
                        Manager = new User
                        {
                            FirstName = userDTORegister.FirstName !,
                            LastName = userDTORegister.LastName !,
                            PhoneNumber = phoneNumberValue
                        }
                    };

                    _dbContext.Houses.Add(newHouse);

                    newHouse.Manager.HouseId = newHouse.Id;

                    _dbContext?.Users?.Add(newHouse.Manager);

                    var userHouse = new UserHouse
                    {
                        UserId = newHouse.Manager.UserId,
                        User = newHouse.Manager,  
                        HouseId = newHouse.Id,
                        House = newHouse          
                    };

                    if (_dbContext != null)
                    {
                        _dbContext.UserHouses.Add(userHouse);
                        _dbContext.SaveChanges();
                        return true;
                    }

                    return false;
                }

              return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to register and save in db.");
                return false;
            }
        }
    }

    public bool PhoneNumberExistInDb(int PhoneNumber)
    {
        if (_dbContext.Users != null)
        {
            return _dbContext.Users.Any(user => user.PhoneNumber == PhoneNumber);
        }

        return false;
    }

    public string GenerateUserToken(string phoneNumber)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "General-User"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("PhoneNumber", phoneNumber)
        };

        return GenereateTokenByClaim(claims);
    }

    private string GenereateTokenByClaim(Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(45),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string getPhoneNumberFromToken(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
        var phoneNumberClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "PhoneNumber");
       
        if (phoneNumberClaim == null || string.IsNullOrEmpty(phoneNumberClaim.Value))
        {
            throw new InvalidOperationException("Phone number claim not found in token");
        }

        return phoneNumberClaim?.Value!;
    }

    public async Task<bool> AddNewHouseSuccsesfully(string token, bool mergeToNewHouse, int managerPhone)
    {
        try
        {
            string phoneNumber = getPhoneNumberFromToken(token);
            User? user = getUserFromPhoneNumber(int.Parse(phoneNumber));

            if (user == null)
            {
                return false;
            }

           await sendRequestToRabbitMQ(user, mergeToNewHouse, managerPhone,int.Parse(phoneNumber));

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in AddNewHouseSuccsesfully");
            
            return false;
        }
    }

    private Task sendRequestToRabbitMQ(User user, bool mergeToNewHouse, int managerPhone, int phoneNumber)
    {
        Mapper mapper = Mapper.Instance;
        UserDTO userDTO = mapper.CreateAUserDtoFromUserObject(user);
        LoginComunicationDWrapper loginComunicationDWrapper = new(userDTO, mergeToNewHouse, managerPhone, phoneNumber);

        RabbitMqHandler rabbitMqHandler = RabbitMqHandler.Instance;
        rabbitMqHandler.SendMassage(loginComunicationDWrapper);

        return Task.CompletedTask;
    }

    private User? getUserFromPhoneNumber(int phoneNumber)
    {
        if (_dbContext.Users == null)
        {
             throw new InvalidOperationException("User table not exist");
        }

        return _dbContext.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
    }
}