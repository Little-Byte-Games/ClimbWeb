using Climb.Data;
using Climb.Requests;
using Climb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag.Annotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Climb.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Climb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger logger;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.configuration = configuration;
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
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await emailSender.SendEmailConfirmationAsync(request.Email, callbackUrl);

                    await signInManager.SignInAsync(user, false);
                    return Ok(user);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest();
        }

        [HttpPost("/api/v1/account/logIn")]
        [SwaggerResponse(HttpStatusCode.BadRequest, null)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string), IsNullable = false)]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, true, false);
            if (result.Succeeded)
            {
                logger.LogInformation("User logged in.");

                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, email)
                };

                var credentials = new SigningCredentials(configuration.GetSecurityKey(), SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "climb.com",
                    audience: "climb",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new {token = serializedToken});
            }

            logger.LogInformation("User login failed.");
            return BadRequest();
        }

        [Authorize]
        [HttpGet("/api/v1/account/test")]
        public IActionResult Test()
        {
            return Ok("Authorized!");
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