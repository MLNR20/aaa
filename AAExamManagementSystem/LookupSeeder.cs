using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem;

public static class LookupSeeder
{
    private static readonly string[] DefaultDepartments =
    {
        "Marketing",
        "Customer Support",
        "Web Development",
        "Art and Design",
        "Photo Video",
        "Content Development"
    };

    // Ensures the fixed lookup rows the UI depends on (e.g. question types) and the default departments exist.
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedQuestionTypesAsync(db);
        await SeedDepartmentsAsync(db);
    }

    private static async Task SeedQuestionTypesAsync(ApplicationDbContext db)
    {
        var existing = await db.QuestionTypes.Select(qt => qt.Name).ToListAsync();
        var missing = QuestionTypeNames.All.Except(existing).ToList();
        if (missing.Count == 0)
        {
            return;
        }

        db.QuestionTypes.AddRange(missing.Select(name => new QuestionType { Name = name }));
        await db.SaveChangesAsync();
    }

    // Adds any default department not already present (matched by name, ignoring case).
    private static async Task SeedDepartmentsAsync(ApplicationDbContext db)
    {
        var existing = await db.Departments.Select(d => d.Name).ToListAsync();
        var missing = DefaultDepartments.Except(existing, StringComparer.OrdinalIgnoreCase).ToList();
        if (missing.Count == 0)
        {
            return;
        }

        db.Departments.AddRange(missing.Select(name => new Department { Name = name }));
        await db.SaveChangesAsync();
    }
}
