using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AAExamManagementSystem.Pages.Questions;

public class EditModel : PageModel
{
    private readonly QuestionService _questionService;

    public EditModel(QuestionService questionService)
    {
        _questionService = questionService;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public QuestionCreateUpdateDto Question { get; set; } = new();

    public SelectList QuestionTypeOptions { get; set; } = null!;
    public SelectList SectionOptions { get; set; } = null!;
    public IList<int> ChoiceTypeIds { get; set; } = new List<int>();

    public async Task<IActionResult> OnGetAsync()
    {
        var question = await _questionService.GetAsync(Id);
        if (question is null)
        {
            return NotFound();
        }

        Question = new QuestionCreateUpdateDto
        {
            QuestionTypeId = question.QuestionTypeId,
            SectionId = question.SectionId,
            QuestionTitle = question.QuestionTitle,
            Image = question.Image,
            Score = question.Score,
            IsUpToEvaluation = question.IsUpToEvaluation,
            IsActive = question.IsActive,
            Choices = question.Choices
        };
        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _questionService.ValidateAsync(Question, ModelState, nameof(Question));
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        if (!await _questionService.UpdateAsync(Id, Question))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Question updated successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        QuestionTypeOptions = await _questionService.GetQuestionTypeOptionsAsync();
        SectionOptions = await _questionService.GetSectionOptionsAsync();
        ChoiceTypeIds = await _questionService.GetChoiceTypeIdsAsync();
    }
}
