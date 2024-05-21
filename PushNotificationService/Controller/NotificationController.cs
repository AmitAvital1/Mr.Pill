using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using MrPill.DTOs.DTOs;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly FirebaseManager _firebaseManager;

    public NotificationController(FirebaseManager firebaseManager)
    {
        _firebaseManager = firebaseManager;
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDTO request)
    {
        try
        {
            // Send the message asynchronously
            var response =  await _firebaseManager.SendNotification(request.Token, request.Title, request.Body);
            
            // Check the response status
            if (response != null)
            {
                return Ok("Message sent successfully: " + response);
                // Handle success
            }
            else
            {
                return StatusCode(500, "Failed to send message: response is null ");
            }
        }
        catch (FirebaseMessagingException ex)
        {
            return StatusCode(500, "Firebase messaging exception: " + ex.ErrorCode + " " + ex.Message);
            
            // // Check for specific error codes and handle accordingly
            // if (ex.ErrorCode == ErrorCode.InvalidArgument)
            // {
            //     // Handle invalid argument error
            // }
            // else if (ex.ErrorCode == ErrorCode.NotFound)
            // {
            //     // Handle not found error
            // }
            // else
            // {
            //     // Handle other errors
            // }
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }
}
