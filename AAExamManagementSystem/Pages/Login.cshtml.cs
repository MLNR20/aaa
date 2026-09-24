using System.ComponentModel.DataAnnotations;
using AAExamManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(SafeReturnUrl());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = Input.UserName.Contains('@')
                ? await _userManager.FindByEmailAsync(Input.UserName)
                : await _userManager.FindByNameAsync(Input.UserName);

            if (user is not null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserId} signed in.", user.Id);
                    return LocalRedirect(SafeReturnUrl());
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {UserId} is locked out.", user.Id);
                    ModelState.AddModelError(string.Empty, "This account is temporarily locked. Please try again later.");
                    return Page();
                }
            }

            // Same message for unknown user and wrong password, so accounts can't be enumerated.
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        private string SafeReturnUrl() =>
            !string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : Url.Content("~/");

        public class LoginInput
        {
            [Required]
            [Display(Name = "Username or email")]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }
    }
}
