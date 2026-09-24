using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: authenticate with SignInManager once user accounts are in place.
            return RedirectToPage("/Index");
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
