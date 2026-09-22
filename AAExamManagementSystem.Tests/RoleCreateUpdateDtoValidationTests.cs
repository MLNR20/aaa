using System.ComponentModel.DataAnnotations;
using AAExamManagementSystem.Models.Dtos;

namespace AAExamManagementSystem.Tests;

public class RoleCreateUpdateDtoValidationTests
{
    private static IList<ValidationResult> Validate(RoleCreateUpdateDto dto)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        return results;
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Course Coordinator")]
    [InlineData("Role123")]
    public void Validate_ValidNames_ProducesNoErrors(string name)
    {
        var dto = new RoleCreateUpdateDto { Name = name };

        var results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_EmptyName_IsRequired()
    {
        var dto = new RoleCreateUpdateDto { Name = string.Empty };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RoleCreateUpdateDto.Name)));
    }

    [Theory]
    [InlineData("Admin!")]
    [InlineData("Admin@2024")]
    [InlineData("Instructor#1")]
    [InlineData("Role$Name")]
    [InlineData("Role_Name")]
    [InlineData("Role-Name")]
    public void Validate_NameWithSpecialCharacters_FailsRegexValidation(string name)
    {
        var dto = new RoleCreateUpdateDto { Name = name };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RoleCreateUpdateDto.Name)));
    }

    [Fact]
    public void Validate_NameTooShort_FailsLengthValidation()
    {
        var dto = new RoleCreateUpdateDto { Name = "A" };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RoleCreateUpdateDto.Name)));
    }

    [Fact]
    public void Validate_NameTooLong_FailsLengthValidation()
    {
        var dto = new RoleCreateUpdateDto { Name = new string('A', 257) };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RoleCreateUpdateDto.Name)));
    }
}
