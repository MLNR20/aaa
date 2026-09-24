namespace AAExamManagementSystem.Models.Entities;

public enum NotificationStatus
{
    Unread = 0,
    Read = 1,
    Archived = 2
}

public class Notification
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string Header { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

    public DateTime? DateRead { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
