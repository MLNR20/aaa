using AAExamManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Pages.Analytics;

public record StatTile(string Label, int Total, int Active, string Icon);

public record BarItem(string Label, int Value);

public record UpcomingExam(string Title, string Course, DateTime ExamDate, int DurationMinutes);

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<StatTile> Tiles { get; private set; } = new List<StatTile>();
    public IList<BarItem> UsersByDepartment { get; private set; } = new List<BarItem>();
    public IList<BarItem> QuestionsBySection { get; private set; } = new List<BarItem>();
    public IList<BarItem> QuestionsByType { get; private set; } = new List<BarItem>();
    public IList<BarItem> AttemptsByMonth { get; private set; } = new List<BarItem>();
    public IList<UpcomingExam> UpcomingExams { get; private set; } = new List<UpcomingExam>();
    public double? AverageScore { get; private set; }

    public async Task OnGetAsync()
    {
        Tiles = new List<StatTile>
        {
            new("Departments", await _context.Departments.CountAsync(), await _context.Departments.CountAsync(x => x.IsActive), "bi-building"),
            new("Sections", await _context.Sections.CountAsync(), await _context.Sections.CountAsync(x => x.IsActive), "bi-diagram-3"),
            new("Courses", await _context.Courses.CountAsync(), await _context.Courses.CountAsync(x => x.IsActive), "bi-journal-bookmark"),
            new("Questions", await _context.Questions.CountAsync(), await _context.Questions.CountAsync(x => x.IsActive), "bi-question-circle"),
            new("Exams", await _context.Exams.CountAsync(), await _context.Exams.CountAsync(x => x.IsActive), "bi-clipboard-check"),
            new("Applicants", await _context.Applicants.CountAsync(), await _context.Applicants.CountAsync(x => x.IsActive), "bi-person-lines-fill"),
            new("Users", await _context.Users.CountAsync(), await _context.Users.CountAsync(u => u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.UtcNow), "bi-people"),
            new("Attempts", await _context.Attempts.CountAsync(), await _context.Attempts.CountAsync(x => x.IsActive), "bi-graph-up")
        };

        UsersByDepartment = await _context.Departments
            .OrderByDescending(d => d.Users.Count).ThenBy(d => d.Name)
            .Select(d => new BarItem(d.Name, d.Users.Count))
            .Take(10)
            .ToListAsync();

        QuestionsBySection = await _context.Sections
            .OrderByDescending(s => s.Questions.Count).ThenBy(s => s.Name)
            .Select(s => new BarItem(s.Name, s.Questions.Count))
            .Take(10)
            .ToListAsync();

        QuestionsByType = await _context.QuestionTypes
            .OrderByDescending(t => t.Questions.Count).ThenBy(t => t.Name)
            .Select(t => new BarItem(t.Name, t.Questions.Count))
            .ToListAsync();

        // Last six calendar months, including months with zero attempts.
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5);
        var monthly = await _context.Attempts
            .Where(a => a.DateCreated >= start)
            .GroupBy(a => new { a.DateCreated.Year, a.DateCreated.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();
        AttemptsByMonth = Enumerable.Range(0, 6)
            .Select(i => start.AddMonths(i))
            .Select(m => new BarItem(m.ToString("MMM yyyy"),
                monthly.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0))
            .ToList();

        AverageScore = await _context.Attempts.AnyAsync()
            ? await _context.Attempts.AverageAsync(a => (double)a.TotalScore)
            : null;

        UpcomingExams = await _context.Exams
            .Where(e => e.IsActive && e.ExamDate >= now.Date)
            .OrderBy(e => e.ExamDate)
            .Take(5)
            .Select(e => new UpcomingExam(e.Title, e.Course.Name, e.ExamDate, e.DurationMinutes))
            .ToListAsync();
    }
}
