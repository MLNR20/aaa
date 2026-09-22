using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AAExamManagementSystem.Pages.Questions;

public class EditModel : PageModel
{
    private readonly IGenericRepository<Question> _repository;
    private readonly IGenericRepository<Exam> _examRepository;
    private readonly IMapper _mapper;

    public EditModel(IGenericRepository<Question> repository, IGenericRepository<Exam> examRepository, IMapper mapper)
    {
        _repository = repository;
        _examRepository = examRepository;
        _mapper = mapper;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public QuestionCreateUpdateDto Question { get; set; } = new();

    public SelectList ExamOptions { get; set; } = new(new List<Exam>(), "Id", "Title");

    public async Task<IActionResult> OnGetAsync()
    {
        var question = await _repository.GetByIdAsync(Id);
        if (question is null)
        {
            return NotFound();
        }

        Question = new QuestionCreateUpdateDto
        {
            ExamId = question.ExamId,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            OptionA = question.OptionA,
            OptionB = question.OptionB,
            OptionC = question.OptionC,
            OptionD = question.OptionD,
            CorrectAnswer = question.CorrectAnswer,
            Points = question.Points,
            IsActive = question.IsActive
        };
        await LoadExamOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadExamOptionsAsync();
            return Page();
        }

        var question = await _repository.GetByIdAsync(Id);
        if (question is null)
        {
            return NotFound();
        }

        question.ExamId = Question.ExamId;
        question.QuestionText = Question.QuestionText;
        question.QuestionType = Question.QuestionType;
        question.OptionA = Question.OptionA;
        question.OptionB = Question.OptionB;
        question.OptionC = Question.OptionC;
        question.OptionD = Question.OptionD;
        question.CorrectAnswer = Question.CorrectAnswer;
        question.Points = Question.Points;
        question.IsActive = Question.IsActive;
        _repository.Update(question);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = "Question updated successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadExamOptionsAsync()
    {
        var exams = await _examRepository.GetAllAsync();
        ExamOptions = new SelectList(exams.OrderBy(e => e.Title), "Id", "Title");
    }
}
