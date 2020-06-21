using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    // 'Static' keyword meaning we don't need to create a new instance
    // of the class Extensions, when we want to use its methods.
    public static class Extensions
    {
        // Overwrite response by passing 'this HttpResponse response'
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            // Add additional headers onto our response. In the event of an exception,
            // when we send this back to our client, there will be these new headers,
            // containing these error messages as their values. 
            response.Headers.Add("Application-Error", message);

            // CORS Header so that angular application doesn't send error because it
            // doesn't have the appropriate access control allow origin etc.
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");

            // Uses wildcard to effectively allow any origin.
            response.Headers.Add("Access-Control-Allow-Origin", "*");

        }
    }
}