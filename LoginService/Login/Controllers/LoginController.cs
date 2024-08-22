using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Login.Models.LoginService;
using MrPill.DTOs.DTOs;

[Authorize] 
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly ILoginService _loginService;
    private readonly object _lockerRegister = new();

    public LoginController(ILogger<LoginController> logger, ILoginService loginService)
    {
        _logger = logger;
        _loginService = loginService;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("Health")]
    public IActionResult Health()
    {
        return Ok("arrive!");
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Body")]
    public IActionResult Body([FromBody] string name)
    {
        Console.WriteLine(name);
        return Ok($"arrive! : {name}");
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public IActionResult Login([FromBody] PhoneNumberDTO UserLogin)
    {
        try
        {
            _logger.LogInformation("Login attempt with phone number {PhoneNumber}", UserLogin.PhoneNumber);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login attempt with phone number {PhoneNumber}", UserLogin.PhoneNumber);
                return BadRequest(new { Message = "Invalid model state in login" });
            }

            if (!int.TryParse(UserLogin.PhoneNumber, out int phoneNumberValue))
            {
                _logger.LogWarning("Invalid phone number format for login attempt with phone number {PhoneNumber}", UserLogin.PhoneNumber);
                return BadRequest(new { Message = "Invalid phone number format" });
            }

            if (_loginService.PhoneNumberExistInDb(phoneNumberValue))
            {
                // Generate 6-digit code
                var code = _loginService.GenerateVerificationCode();
                _logger.LogInformation("validation code: {Code}", code);
                // Save the code and timestamp in the database
                _loginService.SaveVerificationCode(phoneNumberValue, code);

                _logger.LogInformation("Verification code sent to phone number {PhoneNumber}", UserLogin.PhoneNumber);
                return Ok(new { message = "Verification code sent" });
            }

            _logger.LogInformation("User with phone number {PhoneNumber} does not exist", UserLogin.PhoneNumber);
            return NotFound(new { message = "User not found", PhoneNumber = UserLogin.PhoneNumber });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to login with phone number {PhoneNumber}", UserLogin.PhoneNumber);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("ValidateCode")]
    public IActionResult ValidateCode([FromBody] CodeValidationDTO validationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Validating code for phone number {PhoneNumber}", validationDto.PhoneNumber);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for code validation with phone number {PhoneNumber}", validationDto.PhoneNumber);
                return BadRequest(new { Message = "Invalid model state in validation" });
            }

            if (!int.TryParse(validationDto.PhoneNumber, out int phoneNumberValue))
            {
                _logger.LogWarning("Invalid phone number format for validation attempt with phone number {PhoneNumber}", validationDto.PhoneNumber);
                return BadRequest(new { Message = "Invalid phone number format" });
            }

            var isValid = _loginService.ValidateVerificationCode(phoneNumberValue, validationDto.Code);

            if (isValid)
            {
                string UserToken = _loginService.GenerateUserToken(validationDto.PhoneNumber);
                _logger.LogInformation("Code validation successful for phone number {PhoneNumber}", validationDto.PhoneNumber);
                return Ok(new { token = UserToken });
            }

            _logger.LogWarning("Invalid code for phone number {PhoneNumber}", validationDto.PhoneNumber);
            return Unauthorized(new { message = "Invalid code" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while validating the code with phone number {PhoneNumber}", validationDto.PhoneNumber);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("GenerateRegistrationCode")]
    public IActionResult GenerateRegistrationCode([FromBody] PhoneNumberDTO phoneNumberDto)
    {
        try
        {
            _logger.LogInformation("Generating registration code for phone number {PhoneNumber}", phoneNumberDto.PhoneNumber);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for registration code generation with phone number {PhoneNumber}", phoneNumberDto.PhoneNumber);
                return BadRequest(new { Message = "Invalid phone number format" });
            }

            if (!int.TryParse(phoneNumberDto.PhoneNumber, out int phoneNumberValue))
            {
                _logger.LogWarning("Invalid phone number format for registration code generation with phone number {PhoneNumber}", phoneNumberDto.PhoneNumber);
                return BadRequest(new { Message = "Invalid phone number format" });
            }

            bool phoneNumberExistInDb;
            
            lock (_lockerRegister)
            {
                phoneNumberExistInDb = _loginService.PhoneNumberExistInDb(phoneNumberValue);
            }
            if (phoneNumberExistInDb)
            {
                _logger.LogInformation("User with phone number {PhoneNumber} exists in the system", phoneNumberDto.PhoneNumber);
                return Conflict(new { message = "User already exists", PhoneNumber = phoneNumberDto.PhoneNumber });
            }
            
            var code = _loginService.GenerateVerificationCode();
            _logger.LogInformation("validation code: {Code}", code);    
            _loginService.SaveVerificationCode(phoneNumberValue, code);

            _logger.LogInformation("Registration code sent to phone number {PhoneNumber}", phoneNumberDto.PhoneNumber);
            return Ok(new { message = "Registration code sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating registration code for phone number {PhoneNumber}", phoneNumberDto.PhoneNumber);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Register")]
    public IActionResult VerifyAndRegistrationCode([FromBody] CodeValidationRegistrationDTO validationDto)
    {
        try
        {
            _logger.LogInformation("Verifying registration code for phone number {PhoneNumber}", validationDto.PhoneNumber);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for registration code verification with phone number {PhoneNumber}", validationDto.PhoneNumber);
                return BadRequest(ModelState);
            }

            if (!int.TryParse(validationDto.PhoneNumber, out int phoneNumberValue))
            {
                _logger.LogWarning("Invalid phone number format for registration code verification with phone number {PhoneNumber}", validationDto.PhoneNumber);
                return BadRequest(new { Message = "Invalid phone number format" });
            }

            var isValid = _loginService.ValidateVerificationCode(phoneNumberValue, validationDto.Code);

            if (isValid)
            {
                _logger.LogInformation("Registration code validation successful for phone number {PhoneNumber}", validationDto.PhoneNumber);
                var userDTORegister = UserDTO.Builder()
                        .WithFirstName(validationDto.FirstName!)
                        .WithLastName(validationDto.LastName!)
                        .WithPhoneNumber(validationDto.PhoneNumber)
                        .Build();
                
                if (_loginService.RegisterUser(userDTORegister))
                {
                    string UserToken = _loginService.GenerateUserToken(userDTORegister.PhoneNumber!);
                    _logger.LogInformation("User registration successful for phone number {PhoneNumber}", validationDto.PhoneNumber);
                    
                    return Ok(new { token = UserToken });
                }
                else
                {
                    throw new Exception("Failed to register user, Also the number did not exist in the db.");
                }
            }

            _logger.LogWarning("Invalid registration code for phone number {PhoneNumber}", validationDto.PhoneNumber);
            return Unauthorized(new { message = "Invalid code" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while verifying registration code for phone number {PhoneNumber}", validationDto.PhoneNumber);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet]
    [Route("get-notifications")]
    public IActionResult GetNotifications()
    {
        try
        {
            string token = GetTokenFromHeaders();
            IEnumerable<CabinetRequestDTO> cabinetRequestDTOs = _loginService.GetAllRequestByUserToken(token);

            var response = new
            {
                Success = true,
                Message = "Notifications retrieved successfully.",
                TotalNotifications = cabinetRequestDTOs.Count(),
                Timestamp = DateTime.UtcNow,
                Data = cabinetRequestDTOs
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting notifications.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting notifications.");
        }
    }

    [HttpPut]
    [Route("handle-notification")]
    public IActionResult HandleNotification([FromQuery] int requestId, [FromQuery] bool approve)
    {
        try
        {
            string token = GetTokenFromHeaders();
            _loginService.HandleNotification(token, requestId, approve);
            
            var successResponse = new
            {
                Success = true,
                Message = "Notification handled successfully.",
                RequestId = requestId,
                Approved = approve,
                Timestamp = DateTime.UtcNow
            };

            return Ok(successResponse);
        }
        catch (Exception ex)
        {
           _logger.LogError(ex, "An error occurred while handling the notification for request ID {RequestId}.", requestId);

           var errorResponse = new
            {
                Success = false,
                Message = "An error occurred while handling the notification.",
                RequestId = requestId,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            };

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    [HttpPost]
    [Route("joined-new-house")]
    public async Task<IActionResult> JoinedToNewHouse([FromQuery] int targetPhoneNumber, [FromQuery] string medicineCabinetName)
    {
        try
        {
            string token = GetTokenFromHeaders();

            if (!_loginService.PhoneNumberExistInDb(targetPhoneNumber))
            {
                _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database", targetPhoneNumber);
                return NotFound("Phone number does not exist");
            }

            if (await _loginService.AddNewHouseSuccsesfully(token, targetPhoneNumber, medicineCabinetName))
            {
                _logger.LogInformation("Successfully processed request for manager {targetPhoneNumber} to join a new house.", targetPhoneNumber);
                return Ok(new { Massage = "request to joined to another house succsesfuly" });
            }
            
            else
            {
                _logger.LogError("Failed to process request for target Phone Number {targetPhoneNumber} to join a new house.", targetPhoneNumber);
                return StatusCode(500, "Internal Server Error");
            }
        } 
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt: {Message}", ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing the request for target Phone Number {targetPhoneNumber}", targetPhoneNumber);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    private string GetTokenFromHeaders()
    {
        try
        {
            string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token not found in the request headers");
                throw new UnauthorizedAccessException("Token not found in the request headers");
            }

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while extracting the token from headers");
            throw new UnauthorizedAccessException("Failed to extract token", ex);
        }
    }
}
