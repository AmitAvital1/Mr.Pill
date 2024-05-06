using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Login.Models.LoginService;
using MrPill.DTOs.DTOs;

[Authorize]
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly ILoginService _loginService;  

    public LoginController(ILogger <LoginController> logger, ILoginService loginService) 
    {
        _logger = logger;
        _loginService = loginService;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public IActionResult Login([FromBody] UserDTO UserLogin)
    {
        try
        {
            if (ModelState.IsValid)
            {
                string phoneNumberString = UserLogin.PhoneNumber !;
                int phoneNumberValue = int.Parse(phoneNumberString);

                if (_loginService.PhoneNumberExistInDb(phoneNumberValue))
                {
                    string UserToken = _loginService.GenerateUserToken(UserLogin.PhoneNumber!.ToString());
                    return Ok(new { token = UserToken });
                }
                else
                {
                    _logger.LogInformation("User with phone Number {PhoneNumber} does not exist", UserLogin.PhoneNumber);
                    return NotFound(new { message = "User not found", PhoneNumber = UserLogin.PhoneNumber });
                }
            }
            else
            {
                return BadRequest(new { Message = "User not authenticated" });
            }

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
            if (ModelState.IsValid)
            {
                string phoneNumberString = userDTORegister.PhoneNumber !;
                int phoneNumberValue = int.Parse(phoneNumberString);

                if (!_loginService.PhoneNumberExistInDb(phoneNumberValue))
                {
                    if (_loginService.RegisterUser(userDTORegister))
                    {
                        string UserToken = _loginService.GenerateUserToken(userDTORegister.PhoneNumber!.ToString());
                        
                        return Ok(new { token = UserToken });
                    }
                    else
                    {
                        throw new Exception("Failed to register user");
                    }
                }
                else
                {
                    _logger.LogInformation("User with phone Number {PhoneNumber} exist in the system", userDTORegister.PhoneNumber);
                    return NotFound(new { message = "User found", PhoneNumber = userDTORegister.PhoneNumber });
                }
            }
            else
            {
                return BadRequest(new { Message = "User not authenticated" });
            }

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

        if (await _loginService.AddNewHouseSuccsesfully(token, mergeToNewHouse,managerPhone))
        {
            return Ok(new { Massage = "request to joined to another house succsesfuly" });
        }
        else
        {
             return StatusCode(500, "Internal Server Error");
        }
    }
}
