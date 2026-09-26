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
    public bool IsActive { get; set; }
    public DateTime DateCreated { get; set; }
}

public class ApplicantRegisterDto
{
    [Required, StringLength(50, MinimumLength = 3)]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required, StringLength(30)]
    [Display(Name = "Student No.")]
    public string StudentNo { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Year Level")]
    public string? YearLevel { get; set; }

    [StringLength(150)]
    public string? School { get; set; }

    [StringLength(100)]
    [Display(Name = "First Choice")]
    public string? FirstChoice { get; set; }

    [StringLength(100)]
    [Display(Name = "Second Choice")]
    public string? SecondChoice { get; set; }

    [Display(Name = "I am a scholar")]
    public bool IsScholar { get; set; }

    [StringLength(300)]
    [Display(Name = "Resume Link")]
    public string? Resume { get; set; }

    [StringLength(20)]
    [Display(Name = "Expected Graduation Date")]
    public string? ExpectedGradDate { get; set; }

    [Range(0, 20)]
    [Display(Name = "Terms Remaining")]
    public int TermsRemaining { get; set; }
}
