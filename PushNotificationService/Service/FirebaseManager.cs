using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

public class FirebaseManager
{
    private static FirebaseApp? _firebaseApp;
    public FirebaseManager()
    {

         if (_firebaseApp == null)
        {
            _firebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("mrpill-pushnotification-firebase-adminsdk-xx2h6-733f35800d.json"),
            });
        }
    }

    public async Task<string> SendNotification(string token, string title, string body)
    {
        try
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            // Send the message asynchronously
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(false);
            
            // Return the response if successful
            return response;
        }
        catch (FirebaseMessagingException)
        {
            // If FirebaseMessagingException occurs, rethrow it
            throw;
        }
        catch (Exception)
        {
            // If any other exception occurs, rethrow it
            throw;
        }
    }
}