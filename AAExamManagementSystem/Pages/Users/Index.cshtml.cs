using AAExamManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Pages.Users;

public record UserRow(string Id, string UserName, string FullName, string? Email, string? Role);

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IList<UserRow> Users { get; set; } = new List<UserRow>();

    // Active role names offered in the SweetAlert role picker.
    public IList<string> RoleNames { get; set; } = new List<string>();

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            Users.Add(new UserRow(
                user.Id,
                user.UserName ?? string.Empty,
                $"{user.FirstName} {user.LastName}".Trim(),
                user.Email,
                roles.OrderBy(r => r).FirstOrDefault()));
        }

        RoleNames = await _roleManager.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAssignRoleAsync(string id, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var target = await _roleManager.FindByNameAsync(role);
        if (target is null || !target.IsActive)
        {
            TempData["ErrorMessage"] = $"Role '{role}' does not exist or is inactive.";
            return RedirectToPage();
        }

        // An admin can't drop their own Admin role and lock themselves out.
        if (user.Id == _userManager.GetUserId(User)
            && await _userManager.IsInRoleAsync(user, Models.Entities.Roles.Admin)
            && target.Name != Models.Entities.Roles.Admin)
        {
            TempData["ErrorMessage"] = "You cannot remove the Admin role from your own account.";
            return RedirectToPage();
        }

        // One role per user: replace whatever the user currently has.
        var current = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, current);
        if (result.Succeeded)
        {
            result = await _userManager.AddToRoleAsync(user, target.Name!);
        }

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors.Select(e => e.Description));
            return RedirectToPage();
        }

        TempData["SuccessMessage"] = $"'{user.UserName}' is now {target.Name}.";
        return RedirectToPage();
    }
}
