using Microsoft.AspNetCore.Mvc;
namespace UserServiceApp.Controllers;
using UserServiceApp.Models.UserService;
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;

        // Task task = Task.Run(() =>
        // {
        //     sendRequestToGateway();
        // });
    }

    private void sendRequestToGateway()
    {
        try
        {
            var port = HttpContext.Connection.LocalPort;

            // if i have some instances of the server i need to suffly the name of the server
            // need to add a token to the server 
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api-gateway-url.com/endpoint?port={port}")
            };

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Request sent to API gateway successfully");
                }
                else
                {
                    _logger.LogError($"Failed to send request to API gateway. Status code: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while sending request to API gateway: {ex.Message}");
        }
    }
}