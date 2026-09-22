namespace AAExamManagementSystem.Models.Dtos;

public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SectionCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;
}
