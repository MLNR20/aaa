using Microsoft.AspNetCore.Identity;

namespace AAExamManagementSystem.Models.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    public bool IsActive { get; set; } = true;
}
