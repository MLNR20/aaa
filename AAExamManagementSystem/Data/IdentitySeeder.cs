using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using Microsoft.AspNetCore.Identity;

namespace AAExamManagementSystem.Data;

public static class IdentitySeeder
{
    private const string AdminUserName = "admin";
    private const string AdminEmail = "admin@aaexams.local";
    private const string AdminPassword = "P@ssword2026!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }

        foreach (var sectionName in Departments.All)
        {
            var exists = dbContext.Sections.Any(s => s.Name == sectionName);
            if (!exists)
            {
                dbContext.Sections.Add(new Section { Name = sectionName });
            }
        }

        foreach (var questionTypeName in QuestionTypes.All)
        {
            var exists = dbContext.QuestionTypes.Any(qt => qt.Name == questionTypeName);
            if (!exists)
            {
                dbContext.QuestionTypes.Add(new QuestionType { Name = questionTypeName });
            }
        }

        await dbContext.SaveChangesAsync();

        var adminUser = await userManager.FindByNameAsync(AdminUserName);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = AdminUserName,
                Email = AdminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, AdminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }

        await SeedDemoUsersAsync(userManager);
    }

    private const string DemoPassword = "P@ssword2026!";

    private static readonly (string UserName, string FirstName, string LastName, string Role)[] DemoUsers =
    {
        ("jsmith", "John", "Smith", Roles.Instructor),
        ("mgarcia", "Maria", "Garcia", Roles.Instructor),
        ("achen", "Alice", "Chen", Roles.Student),
        ("bwilliams", "Ben", "Williams", Roles.Student),
        ("cjohnson", "Chris", "Johnson", Roles.Student),
        ("dlee", "Diana", "Lee", Roles.Applicant),
        ("ekim", "Ethan", "Kim", Roles.Applicant),
        ("fpatel", "Fatima", "Patel", Roles.Staffer),
        ("gnguyen", "Grace", "Nguyen", Roles.Editor),
        ("hbrown", "Henry", "Brown", Roles.Guest)
    };

    private static async Task SeedDemoUsersAsync(UserManager<ApplicationUser> userManager)
    {
        foreach (var (userName, firstName, lastName, role) in DemoUsers)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = $"{userName}@aaexams.local",
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName
                };

                var result = await userManager.CreateAsync(user, DemoPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to seed demo user '{userName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
