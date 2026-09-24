using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Questions;

public class IndexModel : PageModel
{
    private readonly QuestionService _questionService;

    public IndexModel(QuestionService questionService)
    {
        _questionService = questionService;
    }

    public IList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();

    public async Task OnGetAsync()
    {
        Questions = await _questionService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
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
