using Microsoft.AspNetCore.Mvc;

namespace Climb.Attributes
{
    public class UserToken : FromHeaderAttribute
    {
        public UserToken()
        {
            Name = "Authorization";
        }
    }
}