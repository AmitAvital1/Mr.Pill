using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Login.Models.LoginService;

public class LoginService : ILoginService
{
    private readonly ILogger<LoginService> _logger;
    private readonly IConfiguration _config;

    // todo => inject the object that connected to the db
    // todo => create an entities folder that fullfil the entities User and House
    // todo => support the fanctionality that add a user to another houses  

    public LoginService(ILogger<LoginService> logger,  IConfiguration configuration)
    {
        _logger = logger;
        _config = configuration;
    }

    public bool PhoneNumberExistInDb(int PhoneNumber)
    {
        return true;
    }

    public string GenerateUserToken()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "General-User"),
            new Claim(ClaimTypes.Role, "User"),
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
}