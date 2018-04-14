﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Climb.Data;
using Climb.Requests;
using Climb.Services;
using NSwag.Annotations;

namespace Climb.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger logger;
        private readonly IEmailSender emailSender;

        public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        public IActionResult Test()
        {
            return View();
        }

        [HttpPost("/api/v1/account/register")]
        [SwaggerResponse(typeof(ApplicationUser))]
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

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(user);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest();
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