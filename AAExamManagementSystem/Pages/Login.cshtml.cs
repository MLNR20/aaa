using System.ComponentModel.DataAnnotations;
using AAExamManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = Input.UserName.Contains('@')
                ? await _userManager.FindByEmailAsync(Input.UserName)
                : await _userManager.FindByNameAsync(Input.UserName);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                if (await _userManager.IsInRoleAsync(user, AAExamManagementSystem.Models.Entities.Roles.Applicant))
                {
                    return RedirectToPage("/ApplicantPortal");
                }

                return RedirectToPage("/Index");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "This account has been locked out. Please try again later.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            }

            return Page();
        }

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
