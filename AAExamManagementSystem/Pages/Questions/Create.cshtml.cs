using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AAExamManagementSystem.Pages.Questions;

public class CreateModel : PageModel
{
    private readonly QuestionService _questionService;

    public CreateModel(QuestionService questionService)
    {
        _questionService = questionService;
    }

    [BindProperty]
    public QuestionCreateUpdateDto Question { get; set; } = new();

    public SelectList QuestionTypeOptions { get; set; } = null!;
    public SelectList SectionOptions { get; set; } = null!;
    public IList<int> ChoiceTypeIds { get; set; } = new List<int>();

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _questionService.ValidateAsync(Question, ModelState, nameof(Question));
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        await _questionService.CreateAsync(Question);

        TempData["SuccessMessage"] = "Question created successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        QuestionTypeOptions = await _questionService.GetQuestionTypeOptionsAsync();
        SectionOptions = await _questionService.GetSectionOptionsAsync();
        ChoiceTypeIds = await _questionService.GetChoiceTypeIdsAsync();
    }
}
