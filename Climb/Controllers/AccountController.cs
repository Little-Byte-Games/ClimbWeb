using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Extensions;
using Climb.Requests;
using Climb.Responses;
using Climb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Climb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger logger;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly ITokenHelper tokenHelper;

        public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration, ITokenHelper tokenHelper)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.tokenHelper = tokenHelper;
        }

        [HttpGet("/account/{*page}")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Account";
            ViewData["Script"] = "account";
            return View("~/Views/Page.cshtml");
        }

        [HttpPost("/api/v1/account/register")]
        [SwaggerResponse(HttpStatusCode.BadRequest, null)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ApplicationUser))]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = request.Email, Email = request.Email};
                var result = await userManager.CreateAsync(user, request.Password);
                if(result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await emailSender.SendEmailConfirmationAsync(request.Email, callbackUrl);

                    await signInManager.SignInAsync(user, false);
                    return Ok(user);
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/account/logIn")]
        [SwaggerResponse(HttpStatusCode.BadRequest, null)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LoginResponse), IsNullable = false)]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, true, false);
            if(result.Succeeded)
            {
                logger.LogInformation("User logged in.");

                var token = tokenHelper.CreateUserToken(configuration.GetSecurityKey(), DateTime.Now.AddMinutes(30), email);

                return Ok(new LoginResponse(token));
            }

            logger.LogInformation("User login failed.");
            return BadRequest();
        }

        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        [HttpGet("/api/v1/account/test")]
        public async Task<IActionResult> Test([FromHeader(Name = "Authorization")] string authorization, string userId)
        {
            var authorizedId = await tokenHelper.GetAuthorizedUserID(authorization);

            if(userId == authorizedId)
            {
                return Ok("Authorized!");
            }

            return BadRequest("Not the same user!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            return RedirectToPage("/Index");
        }
    }
}