using Microsoft.AspNetCore.Mvc;

public class ApiGatewayController : Controller
{
    private readonly ILogger<ApiGatewayController> _logger;

    public ApiGatewayController(ILogger<ApiGatewayController> logger)
    {
        _logger = logger;
    }
}