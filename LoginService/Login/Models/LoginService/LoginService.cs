using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Login.Models.DB;
using Microsoft.IdentityModel.Tokens;
using MrPill.DTOs.DTOs;
using Login.Models.MapperObject;
using Login.Models.RabbitMq;
using Microsoft.EntityFrameworkCore;

namespace Login.Models.LoginService;

public class LoginService : ILoginService
{
    private readonly ILogger<LoginService> _logger;
    private readonly IConfiguration _config;
    private readonly AppDbContext _dbContext;
    private static readonly object _lockObjectRegister = new();

    public LoginService(ILogger<LoginService> logger,  IConfiguration configuration, AppDbContext appDbContext)
    {
        _logger = logger;
        _config = configuration;
        _dbContext = appDbContext;
    }

    public bool RegisterUser(UserDTO userDTORegister)
    {
        lock (_lockObjectRegister)
        {
            try
            {
                string phoneNumberString = userDTORegister.PhoneNumber !;
                int phoneNumberValue = int.Parse(phoneNumberString);

                if (!PhoneNumberExistInDb(phoneNumberValue))
                {
                    var newUser = new User
                    {
                        FirstName = userDTORegister.FirstName!,
                        LastName = userDTORegister.LastName!,
                        PhoneNumber = phoneNumberValue
                    };

                    if (_dbContext != null)
                    {
                        _dbContext.Users?.Add(newUser);
                        _dbContext.SaveChanges();

                        return true;
                    }

                    _logger.LogError("Database context is null.");
                    return false;
                }

              return false;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "A database update error occurred while trying to register and save in db.");
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
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
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

    public async Task<bool> AddNewHouseSuccsesfully(string token, int targetPhoneNumber, string medicineCabinetName)
    {
        try
        {
            string phoneNumber = getPhoneNumberFromToken(token);
            User? user = getUserFromPhoneNumber(int.Parse(phoneNumber));

            if (user == null)
            {
                _logger.LogWarning("User with phone number '{PhoneNumber}' not found.", phoneNumber);
                return false;
            }

            var medicineCabinet = user?.MedicineCabinetUsersList
                    ?.FirstOrDefault(mcu => 
                            mcu.MedicineCabinet.MedicineCabinetName.Equals(medicineCabinetName, StringComparison.OrdinalIgnoreCase) &&
                            mcu?.MedicineCabinet?.Creator?.PhoneNumber == user.PhoneNumber);

            if (medicineCabinet == null)
            {
                _logger.LogWarning(
                    "User with phone number {PhoneNumber} is not the creator of the medicine cabinet '{MedicineCabinetName}'.", 
                    phoneNumber, 
                    medicineCabinetName
                );

                return false;
            }

           await sendRequestToRabbitMQ(user!, targetPhoneNumber, int.Parse(phoneNumber), medicineCabinetName);

           _logger.LogInformation(
                "Request to RabbitMQ sent successfully for user with phone number '{PhoneNumber}' and medicine cabinet '{MedicineCabinetName}'.", 
                phoneNumber, 
                medicineCabinetName
            );

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in AddNewHouseSuccsesfully");
            return false;
        }
    }

    private Task sendRequestToRabbitMQ(User user, int targetPhoneNumber, int sourcePhoneNumber, string medicineCabinetName)
    {
        Mapper mapper = Mapper.Instance;
        UserDTO userDTO = mapper.CreateAUserDtoFromUserObject(user);
        LoginComunicationDWrapper loginComunicationDWrapper = new(userDTO, targetPhoneNumber, sourcePhoneNumber, medicineCabinetName);
        RabbitMqHandler rabbitMqHandler = RabbitMqHandler.Instance;
        
        rabbitMqHandler.SendMassage(loginComunicationDWrapper);

        return Task.CompletedTask;
    }

    private User? getUserFromPhoneNumber(int phoneNumber)
    {
        if (_dbContext.Users == null)
        {
            throw new InvalidOperationException("User table does not exist");
        }

        return _dbContext.Users
            ?.Include(u => u.MedicineCabinetUsersList) 
            .ThenInclude(mcu => mcu.MedicineCabinet) 
            .FirstOrDefault(u => u.PhoneNumber == phoneNumber);
    }
    public string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public void SaveVerificationCode(int phoneNumber, string? code)
    {
        var phoneMessage = new PhoneMessage
        {
            PhoneNumber = phoneNumber,
            SentTime = DateTime.Now,
            Code = code
        };

        _dbContext.PhoneMessages.Add(phoneMessage);
        _dbContext.SaveChanges();
    }

    public bool ValidateVerificationCode(int phoneNumber, string? code)
    {
        var latestMessage = _dbContext.PhoneMessages
            .Where(m => m.PhoneNumber == phoneNumber)
            .OrderByDescending(m => m.SentTime)
            .FirstOrDefault();

        if (latestMessage == null || latestMessage.Code != code)
        {
            return false;
        }

        var timeElapsed = DateTime.UtcNow - latestMessage.SentTime;

        if (timeElapsed > TimeSpan.FromMinutes(4))
        {
            return false;
        }

        return true;
    }
}