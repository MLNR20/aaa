using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Questions;

public class DeleteModel : PageModel
{
    private readonly IGenericRepository<Question> _repository;
    private readonly IGenericRepository<Exam> _examRepository;
    private readonly IMapper _mapper;

    public DeleteModel(IGenericRepository<Question> repository, IGenericRepository<Exam> examRepository, IMapper mapper)
    {
        _repository = repository;
        _examRepository = examRepository;
        _mapper = mapper;
    }

    public QuestionDto Question { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        Question = _mapper.Map<QuestionDto>(question);
        var exam = await _examRepository.GetByIdAsync(question.ExamId);
        Question.ExamTitle = exam?.Title ?? "—";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        _repository.Remove(question);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = "Question deleted successfully.";
        return RedirectToPage("Index");
    }
}
