using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public DetailsModel(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public UserDto TargetUser { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        TargetUser = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        TargetUser.Role = roles.Count > 0 ? string.Join(", ", roles) : "-";
        TargetUser.IsActive = !(user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow);

        return Page();
    }
}
