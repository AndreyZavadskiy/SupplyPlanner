using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SP.Data;
using SP.Web.Utility;

namespace SP.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLogger _appLogger;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager, 
            ILogger<LogoutModel> logger, 
            UserManager<ApplicationUser> userManager,
            IAppLogger appLogger)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _appLogger = appLogger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "authorization", "Выход из системы.");

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
