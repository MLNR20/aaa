using System.ComponentModel.DataAnnotations;
using AAExamManagementSystem.Models.Entities;

namespace AAExamManagementSystem.Models.Dtos;

public class QuestionDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? OptionA { get; set; }
    public string? OptionB { get; set; }
    public string? OptionC { get; set; }
    public string? OptionD { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool IsActive { get; set; } = true;
}

public class QuestionCreateUpdateDto
{
    [Required]
    [Display(Name = "Exam")]
    public int ExamId { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Question Text")]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Question Type")]
    public QuestionType QuestionType { get; set; } = QuestionType.MultipleChoice;

    [StringLength(300)]
    [Display(Name = "Option A")]
    public string? OptionA { get; set; }

    [StringLength(300)]
    [Display(Name = "Option B")]
    public string? OptionB { get; set; }

    [StringLength(300)]
    [Display(Name = "Option C")]
    public string? OptionC { get; set; }

    [StringLength(300)]
    [Display(Name = "Option D")]
    public string? OptionD { get; set; }

    [Required, StringLength(300)]
    [Display(Name = "Correct Answer")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
