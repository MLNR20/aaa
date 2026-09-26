namespace AAExamManagementSystem.Models.Entities;

public static class QuestionTypes
{
    public const string MultipleChoice = "Multiple Choice";
    public const string Essay = "Essay";
    public const string TextBased = "Text-Based";
    public const string TrueOrFalse = "True or False";
    public const string Identification = "Identification";

    public static readonly string[] All =
    {
        MultipleChoice, Essay, TextBased, TrueOrFalse, Identification
    };

    public static bool RequiresChoices(string? questionTypeName) =>
        string.Equals(questionTypeName, MultipleChoice, StringComparison.OrdinalIgnoreCase);
}
