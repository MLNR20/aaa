using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem;

public static class LookupSeeder
{
    // Ensures the fixed lookup rows the UI depends on (e.g. question types) exist.
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var existing = await db.QuestionTypes.Select(qt => qt.Name).ToListAsync();
        var missing = QuestionTypeNames.All.Except(existing).ToList();
        if (missing.Count == 0)
        {
            return;
        }

        db.QuestionTypes.AddRange(missing.Select(name => new QuestionType { Name = name }));
        await db.SaveChangesAsync();
    }
}
