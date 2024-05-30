using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Login.Models.LoginService;
using MrPill.DTOs.DTOs;

[Authorize]
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly ILoginService _loginService;
    private readonly object _locker = new();

    public LoginController(ILogger<LoginController> logger, ILoginService loginService)
    {
        _logger = logger;
        _loginService = loginService;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Health")]

    public IActionResult Health()
    {
        return Ok("arrive!");
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public IActionResult Login([FromBody] UserDTO UserLogin)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state in login" });
            }

            if (!int.TryParse(UserLogin.PhoneNumber, out int phoneNumberValue))
            {
                return BadRequest(new { Message = "Invalid phone number format" });
            }

            if (_loginService.PhoneNumberExistInDb(phoneNumberValue))
            {
                string UserToken = _loginService.GenerateUserToken(UserLogin.PhoneNumber);
                return Ok(new { token = UserToken });
            }

            _logger.LogInformation("User with phone number {PhoneNumber} does not exist", UserLogin.PhoneNumber);
            return NotFound(new { message = "User not found", PhoneNumber = UserLogin.PhoneNumber });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to login.");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Register")]
    public IActionResult Register([FromBody] UserDTO userDTORegister)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state In register" });
            }

            if (!int.TryParse(userDTORegister.PhoneNumber, out int phoneNumberValue))
            {
                return BadRequest(new { Message = "Invalid phone number format" });
            }
            
            bool phoneNumberExistInDb;
            
            lock (_locker)
            {
                phoneNumberExistInDb = _loginService.PhoneNumberExistInDb(phoneNumberValue);
            }

            if (!phoneNumberExistInDb)
            {
                if (_loginService.RegisterUser(userDTORegister))
                {
                    string UserToken = _loginService.GenerateUserToken(userDTORegister.PhoneNumber);

                    return Ok(new { token = UserToken });
                }
                else
                {
                    throw new Exception("Failed to register user");
                }
            }

            _logger.LogInformation("User with phone number {PhoneNumber} exists in the system", userDTORegister.PhoneNumber);
            return Conflict(new { message = "User already exists", PhoneNumber = userDTORegister.PhoneNumber });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to register.");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    [Route("joined-new-house")]
    public async Task<IActionResult> JoinedToNewHouse([FromQuery] bool mergeToNewHouse, int managerPhone)
    {
        string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token not found in the request headers");
            return Unauthorized("Token not found");
        }

        if (!_loginService.PhoneNumberExistInDb(managerPhone))
        {
            _logger.LogInformation("Phone number {PhoneNumber} does not exist in the database", managerPhone);
            return NotFound("Phone number does not exist");
        }

        if (await _loginService.AddNewHouseSuccsesfully(token, mergeToNewHouse, managerPhone))
        {
            return Ok(new { Massage = "request to joined to another house succsesfuly" });
        }
        
        else
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
