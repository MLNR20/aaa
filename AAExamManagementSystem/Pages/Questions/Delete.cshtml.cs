using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Questions;

public class DeleteModel : PageModel
{
    private readonly QuestionService _questionService;

    public DeleteModel(QuestionService questionService)
    {
        _questionService = questionService;
    }

    public QuestionDto Question { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var question = await _questionService.GetAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        Question = question;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var (found, error) = await _questionService.DeleteAsync(id);
        if (!found)
        {
            return NotFound();
        }

        if (error is not null)
        {
            TempData["ErrorMessage"] = error;
            return RedirectToPage("Index");
        }

        TempData["SuccessMessage"] = "Question deleted successfully.";
        return RedirectToPage("Index");
    }
}
