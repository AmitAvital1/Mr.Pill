using Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Login.Models.LoginService;
using MrPill.DTOs;
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
                if (_loginService.PhoneNumberExistInDb(UserLogin.PhoneNumber))
                {
                    string UserToken = _loginService.GenerateUserToken();
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
}
