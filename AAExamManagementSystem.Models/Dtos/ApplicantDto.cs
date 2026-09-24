using System.ComponentModel.DataAnnotations;

namespace AAExamManagementSystem.Models.Dtos;

public class ApplicantDto
{
    public int Id { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? FirstChoice { get; set; }
    public string? SecondChoice { get; set; }
    public string? YearLevel { get; set; }
    public string? School { get; set; }
    public bool IsScholar { get; set; }
    public string? Resume { get; set; }
    public string? ExpectedGradDate { get; set; }
    public int TermsRemaining { get; set; }
    public DateTime DateCreated { get; set; }
    public bool IsActive { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public class ApplicantRegistrationDto
{
    [Required, StringLength(50)]
    [Display(Name = "Student No.")]
    public string StudentNo { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Use the format YYYY-YYYY, e.g. 2026-2027.")]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "First Choice")]
    public string? FirstChoice { get; set; }

    [StringLength(150)]
    [Display(Name = "Second Choice")]
    public string? SecondChoice { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Year Level")]
    public string? YearLevel { get; set; }

    [Required, StringLength(200)]
    public string? School { get; set; }

    [Display(Name = "I am a scholar")]
    public bool IsScholar { get; set; }

    [StringLength(20)]
    [Display(Name = "Expected Graduation")]
    public string? ExpectedGradDate { get; set; }

    [Range(0, 20)]
    [Display(Name = "Terms Remaining")]
    public int TermsRemaining { get; set; }
}
