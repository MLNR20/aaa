using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Pages;

// Public applicant sign-up. Each submission is saved as an Applicant record.
[AllowAnonymous]
[RequestSizeLimit(MaxResumeBytes + 1024 * 1024)]
public class RegisterModel : PageModel
{
    public const long MaxResumeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedResumeExtensions = { ".pdf", ".doc", ".docx" };

    public static readonly string[] YearLevels =
    {
        "Grade 12", "1st Year", "2nd Year", "3rd Year", "4th Year", "5th Year", "Graduate"
    };

    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(ApplicationDbContext db, IMapper mapper, IWebHostEnvironment environment, ILogger<RegisterModel> logger)
    {
        _db = db;
        _mapper = mapper;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public ApplicantRegistrationDto Input { get; set; } = new();

    [BindProperty]
    public IFormFile? ResumeFile { get; set; }

    public SelectList CourseOptions { get; set; } = null!;
    public SelectList YearLevelOptions { get; set; } = null!;

    [TempData]
    public string? RegisteredName { get; set; }

    public async Task OnGetAsync()
    {
        Input.AcademicYear = DefaultAcademicYear();
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Email = Input.Email.Trim();
        Input.StudentNo = Input.StudentNo.Trim();

        if (!string.IsNullOrEmpty(Input.SecondChoice) && Input.SecondChoice == Input.FirstChoice)
        {
            ModelState.AddModelError("Input.SecondChoice", "Second choice must be different from your first choice.");
        }

        if (ResumeFile is not null)
        {
            var extension = Path.GetExtension(ResumeFile.FileName).ToLowerInvariant();
            if (!AllowedResumeExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(ResumeFile), "Resume must be a PDF or Word document.");
            }
            else if (ResumeFile.Length == 0 || ResumeFile.Length > MaxResumeBytes)
            {
                ModelState.AddModelError(nameof(ResumeFile), "Resume must be smaller than 5 MB.");
            }
        }

        if (ModelState.IsValid)
        {
            var alreadyRegistered = await _db.Applicants.AnyAsync(a =>
                a.AcademicYear == Input.AcademicYear &&
                (a.StudentNo == Input.StudentNo || a.Email == Input.Email));
            if (alreadyRegistered)
            {
                ModelState.AddModelError(string.Empty,
                    "An application with this student number or email already exists for this academic year.");
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var applicant = _mapper.Map<Applicant>(Input);
        if (ResumeFile is not null)
        {
            applicant.Resume = await SaveResumeAsync(ResumeFile);
        }

        _db.Applicants.Add(applicant);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Applicant {ApplicantId} registered.", applicant.Id);

        RegisteredName = applicant.FirstName;
        return RedirectToPage();
    }

    // Resumes are kept outside wwwroot so they are never served publicly.
    private async Task<string> SaveResumeAsync(IFormFile file)
    {
        var folder = Path.Combine(_environment.ContentRootPath, "App_Data", "resumes");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
        await file.CopyToAsync(stream);
        return fileName;
    }

    private async Task LoadOptionsAsync()
    {
        var courses = await _db.Courses.Where(c => c.IsActive).OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();
        CourseOptions = new SelectList(courses);
        YearLevelOptions = new SelectList(YearLevels);
    }

    // School years start in June, e.g. September 2026 -> "2026-2027".
    private static string DefaultAcademicYear()
    {
        var today = DateTime.Today;
        var start = today.Month >= 6 ? today.Year : today.Year - 1;
        return $"{start}-{start + 1}";
    }
}
