namespace AAExamManagementSystem.Models.Entities;

public class Choice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ChoiceText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<QuestionAndChoice> QuestionAndChoices { get; set; } = new List<QuestionAndChoice>();
}
