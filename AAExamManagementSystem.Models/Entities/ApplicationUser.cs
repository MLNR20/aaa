using Microsoft.AspNetCore.Identity;

namespace AAExamManagementSystem.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Title { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int? SectionId { get; set; }
    public Section? Section { get; set; }

    public int? CourseId { get; set; }
    public Course? Course { get; set; }

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
