using Microsoft.AspNetCore.Mvc;

namespace Climb.Controllers
{
    public class UserController : Controller
    {
        [Route("/user/{*page}")]
        public IActionResult Index()
        {
            ViewData["Title"] = "User";
            ViewData["Script"] = "user";
            return View("~/Views/Page.cshtml");
        }
    }
}