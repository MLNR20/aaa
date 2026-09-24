namespace AAExamManagementSystem.Models.Entities;

public class Applicant
{
    public int Id { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? FirstChoice { get; set; }
    public string? SecondChoice { get; set; }
    public string? YearLevel { get; set; }
    public string? School { get; set; }
    public bool IsScholar { get; set; }
    public string? Resume { get; set; }
    public string? ExpectedGradDate { get; set; }
    public int TermsRemaining { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
