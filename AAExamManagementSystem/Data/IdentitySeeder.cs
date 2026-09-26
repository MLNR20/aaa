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

        var sections = dbContext.Sections.ToList();

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

        await SeedDemoUsersAsync(userManager, sections);
        await SeedWebDevelopmentQuestionsAsync(dbContext);
    }

    private static readonly (string QuestionTitle, string[] Choices, int CorrectIndex)[] WebDevelopmentQuestions =
    {
        ("What does HTML stand for?",
            new[] { "Hyper Text Markup Language", "High Text Machine Language", "Hyperlink and Text Markup Language", "Home Tool Markup Language" }, 0),
        ("Which CSS property is used to change the text color of an element?",
            new[] { "font-color", "text-color", "color", "background-color" }, 2),
        ("Which HTML tag is used to define an internal style sheet?",
            new[] { "<css>", "<script>", "<style>", "<link>" }, 2),
        ("In JavaScript, which keyword declares a block-scoped variable that can be reassigned?",
            new[] { "const", "let", "var", "static" }, 1),
        ("Which HTTP method is typically used to submit data to be processed to a specified resource?",
            new[] { "GET", "POST", "HEAD", "OPTIONS" }, 1),
        ("What does CSS stand for?",
            new[] { "Cascading Style Sheets", "Computer Style Sheets", "Creative Style System", "Colorful Style Sheets" }, 0),
        ("Which of the following is a JavaScript framework/library for building user interfaces?",
            new[] { "Laravel", "Django", "React", "Symfony" }, 2),
        ("Which HTML element is used to create a hyperlink?",
            new[] { "<link>", "<a>", "<href>", "<nav>" }, 1),
        ("Which status code indicates a successful HTTP response?",
            new[] { "200", "301", "404", "500" }, 0),
        ("Which CSS layout module is designed for one-dimensional layouts like rows or columns?",
            new[] { "CSS Grid", "Flexbox", "Float", "Position" }, 1)
    };

    private static async Task SeedWebDevelopmentQuestionsAsync(ApplicationDbContext dbContext)
    {
        var section = dbContext.Sections.FirstOrDefault(s => s.Name == Departments.WebDevelopment);
        if (section is null)
        {
            return;
        }

        var multipleChoiceType = dbContext.QuestionTypes.FirstOrDefault(qt => qt.Name == QuestionTypes.MultipleChoice);
        if (multipleChoiceType is null)
        {
            return;
        }

        foreach (var (questionTitle, choices, correctIndex) in WebDevelopmentQuestions)
        {
            var exists = dbContext.Questions.Any(q => q.QuestionTitle == questionTitle && q.SectionId == section.Id);
            if (exists)
            {
                continue;
            }

            var question = new Question
            {
                QuestionTypeId = multipleChoiceType.Id,
                SectionId = section.Id,
                QuestionTitle = questionTitle,
                Score = 1
            };

            dbContext.Questions.Add(question);

            for (var i = 0; i < choices.Length; i++)
            {
                var choice = new Choice
                {
                    ChoiceText = choices[i],
                    IsCorrect = i == correctIndex
                };

                dbContext.Choices.Add(choice);
                dbContext.QuestionAndChoices.Add(new QuestionAndChoice
                {
                    Question = question,
                    Choice = choice
                });
            }
        }

        await dbContext.SaveChangesAsync();
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

    private static async Task SeedDemoUsersAsync(UserManager<ApplicationUser> userManager, IList<Section> sections)
    {
        var random = new Random();

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
                    LastName = lastName,
                    SectionId = sections.Count > 0 ? sections[random.Next(sections.Count)].Id : null
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
