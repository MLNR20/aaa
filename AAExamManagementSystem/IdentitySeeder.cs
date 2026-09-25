using AAExamManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace AAExamManagementSystem;

public static class IdentitySeeder
{
    // Ensures the built-in roles exist and, if configured, creates the first Admin account.
    // Credentials come from configuration (user secrets / environment), never from source:
    //   SeedAdmin:UserName, SeedAdmin:Email, SeedAdmin:Password
    // In Development it also seeds sample accounts using SeedUsers:Password.
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IdentitySeeder));

        foreach (var roleName in new[] { Roles.Admin, Roles.Staffer })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }

        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            await SeedSampleUsersAsync(userManager, config["SeedUsers:Password"], logger);
        }

        var userName = config["SeedAdmin:UserName"];
        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (await userManager.FindByNameAsync(userName) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator"
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to seed admin user: {Errors}",
                string.Join(" ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);
        logger.LogInformation("Seeded admin user {UserName}.", userName);
    }

    // Development-only sample accounts. They share one password from SeedUsers:Password;
    // existing accounts are left untouched, so re-running is safe.
    private static readonly (string UserName, string FirstName, string LastName, string Role)[] SampleUsers =
    {
        ("admin01", "Maria", "Santos", Roles.Admin),
        ("admin02", "Jose", "Reyes", Roles.Admin),
        ("staffer01", "Ana", "Cruz", Roles.Staffer),
        ("staffer02", "Carlos", "Bautista", Roles.Staffer),
        ("staffer03", "Liza", "Garcia", Roles.Staffer),
        ("staffer04", "Mark", "Villanueva", Roles.Staffer),
        ("staffer05", "Grace", "Mendoza", Roles.Staffer),
        ("staffer06", "Paolo", "Ramos", Roles.Staffer),
        ("staffer07", "Kristine", "Aquino", Roles.Staffer),
        ("staffer08", "Daniel", "Torres", Roles.Staffer)
    };

    private static async Task SeedSampleUsersAsync(UserManager<ApplicationUser> userManager, string? password, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        foreach (var (userName, firstName, lastName, role) in SampleUsers)
        {
            if (await userManager.FindByNameAsync(userName) is not null)
            {
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = $"{userName}@ems.local",
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to seed user {UserName}: {Errors}", userName,
                    string.Join(" ", result.Errors.Select(e => e.Description)));
                continue;
            }

            await userManager.AddToRoleAsync(user, role);
            logger.LogInformation("Seeded {Role} user {UserName}.", role, userName);
        }
    }
}
