namespace AAExamManagementSystem.Models.Entities;

public class QuestionAndChoice
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public int ChoiceId { get; set; }
    public Choice Choice { get; set; } = null!;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
