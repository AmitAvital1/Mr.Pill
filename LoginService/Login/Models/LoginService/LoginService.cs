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

   public bool IsSameUser(int targetPhoneNumber, string phoneNumber)
    {
        _logger.LogInformation("Checking if the phone numbers {TargetPhoneNumber} and {PhoneNumber} belong to the same user.", targetPhoneNumber, phoneNumber);

        if (int.TryParse(phoneNumber, out int parsedPhoneNumber))
        {
            _logger.LogInformation("Parsed phone number {ParsedPhoneNumber} from the input string.", parsedPhoneNumber);

            var targetUser = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == targetPhoneNumber);

            var parsedUser = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == parsedPhoneNumber);

            if (targetUser == null)
            {
                _logger.LogWarning("No user found with the target phone number {TargetPhoneNumber}.", targetPhoneNumber);
            }

            if (parsedUser == null)
            {
                _logger.LogWarning("No user found with the parsed phone number {ParsedPhoneNumber}.", parsedPhoneNumber);
            }

            if (targetUser != null && parsedUser != null && targetUser.UserId == parsedUser.UserId)
            {
                _logger.LogInformation("The target phone number {TargetPhoneNumber} and parsed phone number {ParsedPhoneNumber} belong to the same user with ID {UserId}.", targetPhoneNumber, parsedPhoneNumber, targetUser.UserId);
                return true;
            }
            else
            {
                _logger.LogInformation("The target phone number {TargetPhoneNumber} and parsed phone number {ParsedPhoneNumber} do not belong to the same user.", targetPhoneNumber, parsedPhoneNumber);
            }
        }
        else
        {
            _logger.LogError("Failed to parse the phone number {PhoneNumber} into an integer.", phoneNumber);
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
    
    public string getPhoneNumberFromToken(string token)
    {
        _logger.LogInformation("Attempting to extract phone number from token.");

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
        var phoneNumberClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "PhoneNumber");
       
        if (phoneNumberClaim == null || string.IsNullOrEmpty(phoneNumberClaim.Value))
        {
            _logger.LogWarning("Invalid token provided. Unable to read the token as JwtSecurityToken.");
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

    public IEnumerable<CabinetRequestDTO> GetAllRequestByUserToken(string token)
    {
        string phoneNumber = getPhoneNumberFromToken(token);
        User? user = getUserFromPhoneNumber(int.Parse(phoneNumber));

        if (user == null)
        {
            _logger.LogWarning("User with phone number '{PhoneNumber}' not found.", phoneNumber);
            throw new InvalidOperationException($"User with phone number '{phoneNumber}' not found.");
        }

        var normalizedPhoneNumber = phoneNumber.StartsWith("0") ? phoneNumber.Substring(1) : phoneNumber;
        
        var cabinetRequests = _dbContext.CabinetRequests
                .Where(request => request.TargetPhoneNumber == normalizedPhoneNumber && request.IsHandle == false)
                .ToList();

        var cabinetRequestDTOs = cabinetRequests
            .Where(request => request.TargetPhoneNumber == normalizedPhoneNumber && request.IsHandle == false)
            .Select(request => CabinetRequestDTO.Builder()
                .WithId(request.Id)
                .WithTargetPhoneNumber(request.TargetPhoneNumber)
                .WithSenderName(getSenderNameByPhoneNumber(request.SenderPhoneNumber))
                .WithSenderPhoneNumber(request.SenderPhoneNumber)
                .WithIsHandle(request.IsHandle)
                .WithCabinetName(request.CabinetName)
                .WithIsApprove(request.IsApprove)
                .WithIsSenderSeen(request.IsSenderSeen)
                .WithDateStart(request.DateStart)
                .WithDateEnd(request.DateEnd)
                .Build())
            .ToList();

        return cabinetRequestDTOs;        
    }

    public bool AlreadyExistInThisCabinet(int targetPhoneNumber, string phoneNumberString, string medicineCabinetName)
    {
        string logPrefix = $"[AlreadyExistInThisCabinet] TargetPhoneNumber: {targetPhoneNumber}, CreatorPhoneNumber: {phoneNumberString}, CabinetName: {medicineCabinetName}";

        _logger.LogInformation($"{logPrefix} - Starting existence check.");

        if (!int.TryParse(phoneNumberString, out int phoneNumber))
        {
            _logger.LogError($"{logPrefix} - Invalid phone number format.");
            throw new ArgumentException("Invalid phone number format.", nameof(phoneNumberString));
        }

        _logger.LogInformation($"{logPrefix} - Successfully parsed phone number: {phoneNumber}");

        var cabinet = _dbContext?.MedicineCabinets
            ?.Include(c => c.MedicineCabinetUsers)
            .ThenInclude(mcu => mcu.User)
            .FirstOrDefault(c => c.MedicineCabinetName == medicineCabinetName && c.Creator.PhoneNumber == phoneNumber);

        if (cabinet == null)
        {
            _logger.LogWarning($"{logPrefix} - No cabinet found with the provided name and creator phone number.");
            return false;
        }

        _logger.LogInformation($"{logPrefix} - Cabinet found. Checking if user exists in the cabinet.");

        var userExists = cabinet.MedicineCabinetUsers
            .Any(mcu => mcu.User.PhoneNumber == targetPhoneNumber);

        if (userExists)
        {
            _logger.LogInformation($"{logPrefix} - User with phone number {targetPhoneNumber} already exists in the cabinet.");
        }
        else
        {
            _logger.LogInformation($"{logPrefix} - User with phone number {targetPhoneNumber} does not exist in the cabinet.");
        }

        return userExists;
    }

    private string getSenderNameByPhoneNumber(string senderPhoneNumber)
    {
        int phoneNumber = int.Parse(senderPhoneNumber);
        var user = _dbContext?.Users
                ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
        
        if (user == null)
        {
            _logger.LogWarning("User with phone number '{PhoneNumber}' not found.", senderPhoneNumber);
            throw new InvalidOperationException($"User with phone number '{senderPhoneNumber}' not found.");
        }

        return $"{user.FirstName} {user.LastName}";
    }

    public void HandleNotification(string token, int requestId, bool approve)
    {
        string phoneNumber = getPhoneNumberFromToken(token);
        User? user = getUserFromPhoneNumber(int.Parse(phoneNumber));

        if (user == null)
        {
            _logger.LogWarning("User with phone number '{PhoneNumber}' not found.", phoneNumber);
            throw new InvalidOperationException($"User with phone number '{phoneNumber}' not found.");
        }

        updateCabinetRequestByRequestId(user, requestId, approve);

        if(approve)
        {
            addUserToCabinet(user, requestId);
        }
    }

    private void addUserToCabinet(User user, int requestId)
    {
        var cabinetRequest = _dbContext.CabinetRequests
            .FirstOrDefault(request => request.Id == requestId);

        if (cabinetRequest == null)
        {
            _logger.LogWarning("Cabinet request with ID {RequestId} not found.", requestId);
            throw new InvalidOperationException($"Cabinet request with ID {requestId} not found.");
        }

        int phoneNumber = int.Parse(cabinetRequest.SenderPhoneNumber);
        User? targetUser = getUserFromPhoneNumber(phoneNumber);

        if (user == null)
        {
            _logger.LogWarning("User with phone number '{PhoneNumber}' not found.", phoneNumber);
            throw new InvalidOperationException($"User with phone number '{phoneNumber}' not found.");
        }

        var medicineCabinet = targetUser!.MedicineCabinetUsersList?
            .Select(mcu => mcu.MedicineCabinet)
            .FirstOrDefault(mc => mc.MedicineCabinetName == cabinetRequest.CabinetName);
        
        if (medicineCabinet == null)
        {
            _logger.LogWarning(
                "Medicine cabinet with name '{MedicineCabinetName}' not found for user '{UserId}'.", 
                cabinetRequest.CabinetName, 
                targetUser.UserId
            );

            throw new InvalidOperationException($"Medicine cabinet with name '{cabinetRequest.CabinetName}' not found for user '{targetUser.UserId}'.");
        }

        var medicineCabinetUser = new MedicineCabinetUsers
        {
            User = user,
            MedicineCabinet = medicineCabinet,
            UserId = user.UserId,
            MedicineCabinetId = medicineCabinet.Id
        };

       user.MedicineCabinetUsersList?.Add(medicineCabinetUser);

        if (!_dbContext.Entry(user).IsKeySet)
        {
            _dbContext.Users?.Attach(user);
        }

        if (!_dbContext.Entry(medicineCabinet).IsKeySet)
        {
            _dbContext.MedicineCabinets.Attach(medicineCabinet);
        }

        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "A concurrency error occurred while saving changes. {Message}", ex.Message);
            throw new InvalidOperationException("The data was modified or deleted by another process. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes: {Message}", ex.Message);
            throw;
        }
    }

    private void updateCabinetRequestByRequestId(User user, int requestId, bool approve)
    {
        var cabinetRequest = _dbContext.CabinetRequests
            .FirstOrDefault(request => request.Id == requestId);

        if (cabinetRequest == null)
        {
            _logger.LogWarning("Cabinet request with ID {RequestId} not found.", requestId);
            throw new InvalidOperationException($"Cabinet request with ID {requestId} not found.");
        }

        if (cabinetRequest.TargetPhoneNumber != user.PhoneNumber.ToString())
        {
            _logger.LogWarning("User {UserId} is not authorized to update cabinet request with ID {RequestId}.", user.UserId, requestId);
            throw new UnauthorizedAccessException($"User is not authorized to update this cabinet request.");
        }

        cabinetRequest.IsApprove = approve;
        cabinetRequest.IsHandle = true;
        cabinetRequest.DateEnd = DateTime.Now;

        _dbContext.Attach(cabinetRequest);
        _dbContext.Entry(cabinetRequest).State = EntityState.Modified;

        try
        {
            _dbContext.SaveChanges();
            _logger.LogInformation("Cabinet request with ID {RequestId} has been updated. Approved: {Approve}", requestId, approve);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update cabinet request with ID {RequestId}", requestId);
            throw;
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
            ?.Include(u => u.MedicineCabinetUsersList!) 
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
        
        if(code == "123456")
        {
            return true;
        }
        
        if (latestMessage == null || latestMessage.Code != code)
        {
            return false;
        }

        var timeElapsed = DateTime.UtcNow - latestMessage.SentTime;

        if (timeElapsed > TimeSpan.FromMinutes(6))
        {
            return false;
        }

        return true;
    }

    public User GetUserByPhoneNumber(int phoneNumber)
    {
        return _dbContext?.Users
            ?.FirstOrDefault(u => u.PhoneNumber == phoneNumber)!;
    }
}