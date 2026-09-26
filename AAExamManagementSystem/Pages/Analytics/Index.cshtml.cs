using AppRoles = AAExamManagementSystem.Models.Entities.Roles;
using DepartmentNames = AAExamManagementSystem.Models.Entities.Departments;
using AAExamManagementSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Pages.Analytics;

[Authorize(Roles = AppRoles.Admin + "," + AppRoles.Instructor + "," + AppRoles.Staffer + "," + AppRoles.Editor)]
public class IndexModel : PageModel
{
    private const int MonthsInTrend = 12;
    private const int ScoreBucketCount = 10;

    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int TotalApplicants { get; private set; }
    public int ActiveApplicants { get; private set; }
    public int ScholarApplicants { get; private set; }
    public int TotalAttempts { get; private set; }
    public double? AverageScore { get; private set; }
    public int? HighestScore { get; private set; }
    public int TotalQuestions { get; private set; }
    public int TotalStaff { get; private set; }

    public IList<LabeledCount> ApplicantsPerMonth { get; private set; } = new List<LabeledCount>();
    public IList<ChoiceCount> DepartmentChoices { get; private set; } = new List<ChoiceCount>();
    public IList<LabeledCount> QuestionsPerSection { get; private set; } = new List<LabeledCount>();
    public IList<LabeledCount> QuestionsPerType { get; private set; } = new List<LabeledCount>();
    public IList<LabeledCount> ScoreDistribution { get; private set; } = new List<LabeledCount>();
    public IList<LabeledCount> StaffPerDepartment { get; private set; } = new List<LabeledCount>();
    public IList<TopAttempt> TopAttempts { get; private set; } = new List<TopAttempt>();

    public async Task OnGetAsync()
    {
        var applicants = _db.Applicants.AsNoTracking();
        var attempts = _db.Attempts.AsNoTracking().Where(a => a.IsActive);

        TotalApplicants = await applicants.CountAsync();
        ActiveApplicants = await applicants.CountAsync(a => a.IsActive);
        ScholarApplicants = await applicants.CountAsync(a => a.IsScholar);
        TotalAttempts = await attempts.CountAsync();
        AverageScore = await attempts.AverageAsync(a => (double?)a.TotalScore);
        HighestScore = await attempts.MaxAsync(a => (int?)a.TotalScore);
        TotalQuestions = await _db.Questions.CountAsync(q => q.IsActive);
        TotalStaff = await _db.Users.CountAsync(u => u.Applicant == null);

        await LoadApplicantTrendAsync();
        await LoadDepartmentChoicesAsync();
        await LoadQuestionBreakdownAsync();
        await LoadScoreDistributionAsync();
        await LoadStaffPerDepartmentAsync();

        TopAttempts = await attempts
            .OrderByDescending(a => a.TotalScore)
            .ThenBy(a => a.DateCreated)
            .Take(5)
            .Select(a => new TopAttempt(
                a.Applicant.FirstName + " " + a.Applicant.LastName,
                a.Applicant.StudentNo,
                a.Applicant.FirstChoice,
                a.TotalScore,
                a.DateCreated))
            .ToListAsync();
    }

    private async Task LoadApplicantTrendAsync()
    {
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(MonthsInTrend - 1));

        var counts = await _db.Applicants.AsNoTracking()
            .Where(a => a.DateCreated >= start)
            .GroupBy(a => new { a.DateCreated.Year, a.DateCreated.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();

        ApplicantsPerMonth = Enumerable.Range(0, MonthsInTrend)
            .Select(i => start.AddMonths(i))
            .Select(m => new LabeledCount(
                m.ToString("MMM yyyy"),
                counts.FirstOrDefault(c => c.Year == m.Year && c.Month == m.Month)?.Count ?? 0))
            .ToList();
    }

    private async Task LoadDepartmentChoicesAsync()
    {
        var first = await _db.Applicants.AsNoTracking()
            .Where(a => a.FirstChoice != null && a.FirstChoice != "")
            .GroupBy(a => a.FirstChoice!)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Name, x => x.Count);

        var second = await _db.Applicants.AsNoTracking()
            .Where(a => a.SecondChoice != null && a.SecondChoice != "")
            .GroupBy(a => a.SecondChoice!)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Name, x => x.Count);

        DepartmentChoices = DepartmentNames.All
            .Union(first.Keys)
            .Union(second.Keys)
            .Select(name => new ChoiceCount(name, first.GetValueOrDefault(name), second.GetValueOrDefault(name)))
            .OrderByDescending(c => c.FirstChoice + c.SecondChoice)
            .ThenBy(c => c.Name)
            .ToList();
    }

    private async Task LoadQuestionBreakdownAsync()
    {
        // EF can't translate ordering on a constructor-projected record, so sort an anonymous projection in SQL
        // and build LabeledCount in memory.
        var perSection = await _db.Sections.AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => new { s.Name, Count = s.Questions.Count(q => q.IsActive) })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
        QuestionsPerSection = perSection.Select(x => new LabeledCount(x.Name, x.Count)).ToList();

        var perType = await _db.QuestionTypes.AsNoTracking()
            .Where(t => t.IsActive)
            .Select(t => new { t.Name, Count = t.Questions.Count(q => q.IsActive) })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
        QuestionsPerType = perType.Select(x => new LabeledCount(x.Name, x.Count)).ToList();
    }

    private async Task LoadScoreDistributionAsync()
    {
        var scores = await _db.Attempts.AsNoTracking()
            .Where(a => a.IsActive)
            .Select(a => a.TotalScore)
            .ToListAsync();

        if (scores.Count == 0)
        {
            ScoreDistribution = new List<LabeledCount>();
            return;
        }

        var max = scores.Max();
        var width = Math.Max(1, (int)Math.Ceiling((max + 1) / (double)ScoreBucketCount));
        var buckets = new List<LabeledCount>();
        for (var low = 0; low <= max; low += width)
        {
            var high = low + width - 1;
            var label = width == 1 ? low.ToString() : $"{low}–{high}";
            buckets.Add(new LabeledCount(label, scores.Count(s => s >= low && s <= high)));
        }

        ScoreDistribution = buckets;
    }

    private async Task LoadStaffPerDepartmentAsync()
    {
        var perDepartment = await _db.Departments.AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new { d.Name, Count = d.Users.Count })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Name)
            .ToListAsync();
        StaffPerDepartment = perDepartment.Select(x => new LabeledCount(x.Name, x.Count)).ToList();
    }

    public record LabeledCount(string Label, int Count);

    public record ChoiceCount(string Name, int FirstChoice, int SecondChoice);

    public record TopAttempt(string ApplicantName, string StudentNo, string? FirstChoice, int Score, DateTime DateTaken);
}
