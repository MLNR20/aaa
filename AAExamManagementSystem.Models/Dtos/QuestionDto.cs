using System.ComponentModel.DataAnnotations;

namespace AAExamManagementSystem.Models.Dtos;

public class QuestionDto
{
    public int Id { get; set; }
    public int QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public string QuestionTitle { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int Score { get; set; }
    public bool IsUpToEvaluation { get; set; }
    public bool IsActive { get; set; } = true;
    public IList<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();
}

public class QuestionCreateUpdateDto
{
    [Required]
    [Display(Name = "Question Type")]
    public int QuestionTypeId { get; set; }

    [Required]
    [Display(Name = "Section")]
    public int SectionId { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Question Title")]
    public string QuestionTitle { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Image { get; set; }

    [Range(1, 100)]
    public int Score { get; set; } = 1;

    [Display(Name = "Up for Evaluation")]
    public bool IsUpToEvaluation { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    // Only used by choice-based question types; blank rows are ignored on save.
    public IList<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();
}

public class ChoiceDto
{
    public int Id { get; set; }

    [StringLength(500)]
    [Display(Name = "Choice")]
    public string ChoiceText { get; set; } = string.Empty;

    [Display(Name = "Correct")]
    public bool IsCorrect { get; set; }
}
