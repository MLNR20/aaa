using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Questions;

public class DetailsModel : PageModel
{
    private readonly QuestionService _questionService;

    public DetailsModel(QuestionService questionService)
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
}
