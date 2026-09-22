namespace AAExamManagementSystem.Models.Entities;

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    Essay
}

public class Question
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; } = QuestionType.MultipleChoice;
    public string? OptionA { get; set; }
    public string? OptionB { get; set; }
    public string? OptionC { get; set; }
    public string? OptionD { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}
