namespace AAExamManagementSystem.Models.Entities;

public class Choice
{
    public int Id { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<QuestionAndChoice> QuestionAndChoices { get; set; } = new List<QuestionAndChoice>();
}
