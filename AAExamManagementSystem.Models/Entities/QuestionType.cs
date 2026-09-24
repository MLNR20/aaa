namespace AAExamManagementSystem.Models.Entities;

public class QuestionType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
