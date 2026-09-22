namespace AAExamManagementSystem.Models.Entities;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTime ExamDate { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
}
