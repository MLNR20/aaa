namespace AAExamManagementSystem.Models.Entities;

public static class QuestionTypeNames
{
    public const string MultipleChoice = "Multiple Choice";
    public const string TrueOrFalse = "True or False";
    public const string Identification = "Identification";
    public const string Essay = "Essay";

    public static readonly string[] All = { MultipleChoice, TrueOrFalse, Identification, Essay };

    // Types whose answers are picked from a list of choices.
    public static bool UsesChoices(string? name) =>
        name == MultipleChoice || name == TrueOrFalse;
}
