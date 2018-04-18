namespace Climb.Responses
{
    public class LoginResponse
    {
        public readonly string token;

        public LoginResponse(string token)
        {
            this.token = token;
        }
    }
}