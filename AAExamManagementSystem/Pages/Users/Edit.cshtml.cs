using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Users;

public class EditModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    [BindProperty]
    public UserEditDto TargetUser { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.FindByIdAsync(Id);
        if (user is null)
        {
            return NotFound();
        }

        TargetUser = new UserEditDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            IsActive = !(user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByIdAsync(Id);
        if (user is null)
        {
            return NotFound();
        }

        if (!string.Equals(user.Email, TargetUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(TargetUser.Email);
            if (existing is not null && existing.Id != user.Id)
            {
                ModelState.AddModelError("TargetUser.Email", $"Email '{TargetUser.Email}' is already in use.");
                return Page();
            }

            await _userManager.SetEmailAsync(user, TargetUser.Email);
        }

        user.FirstName = TargetUser.FirstName;
        user.LastName = TargetUser.LastName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await _userManager.SetLockoutEndDateAsync(user, TargetUser.IsActive ? null : DateTimeOffset.MaxValue);

        TempData["SuccessMessage"] = $"User '{user.UserName}' updated successfully.";
        return RedirectToPage("Index");
    }
}
