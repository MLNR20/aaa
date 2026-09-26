using AAExamManagementSystem.Models.Dtos;

namespace AAExamManagementSystem.Pages.Questions;

public record ChoicesEditorModel(
    string Prefix,
    IList<ChoiceCreateUpdateDto> Choices,
    IList<int> ChoiceTypeIds);
