using System.ComponentModel.DataAnnotations;

namespace AAExamManagementSystem.Models.Dtos;

public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SectionCreateUpdateDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
