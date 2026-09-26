using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Users;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public IndexModel(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public IList<UserDto> Users { get; set; } = new List<UserDto>();

    public async Task OnGetAsync()
    {
        await LoadUsersAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        TempData["SuccessMessage"] = result.Succeeded
            ? $"User '{user.UserName}' deleted successfully."
            : string.Join(" ", result.Errors.Select(e => e.Description));

        return RedirectToPage("Index");
    }

    private async Task LoadUsersAsync()
    {
        var users = _userManager.Users.OrderBy(u => u.UserName).ToList();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var dto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.Count > 0 ? string.Join(", ", roles) : "-";
            dto.IsActive = !(user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow);
            userDtos.Add(dto);
        }

        Users = userDtos;
    }
}
