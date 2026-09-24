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
    private readonly IGenericRepository<QuestionType> _questionTypeRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IMapper _mapper;

    public EditModel(
        IGenericRepository<Question> repository,
        IGenericRepository<QuestionType> questionTypeRepository,
        IGenericRepository<Section> sectionRepository,
        IMapper mapper)
    {
        _repository = repository;
        _questionTypeRepository = questionTypeRepository;
        _sectionRepository = sectionRepository;
        _mapper = mapper;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public QuestionCreateUpdateDto Question { get; set; } = new();

    public SelectList QuestionTypeOptions { get; set; } = new(new List<QuestionType>(), "Id", "Name");
    public SelectList SectionOptions { get; set; } = new(new List<Section>(), "Id", "Name");

    public async Task<IActionResult> OnGetAsync()
    {
        var question = await _repository.GetByIdAsync(Id);
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
            IsActive = question.IsActive
        };
        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var question = await _repository.GetByIdAsync(Id);
        if (question is null)
        {
            return NotFound();
        }

        question.QuestionTypeId = Question.QuestionTypeId;
        question.SectionId = Question.SectionId;
        question.QuestionTitle = Question.QuestionTitle;
        question.Image = Question.Image;
        question.Score = Question.Score;
        question.IsUpToEvaluation = Question.IsUpToEvaluation;
        question.IsActive = Question.IsActive;
        question.DateUpdated = DateTime.UtcNow;
        _repository.Update(question);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = "Question updated successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        var questionTypes = await _questionTypeRepository.GetAllAsync();
        var sections = await _sectionRepository.GetAllAsync();
        QuestionTypeOptions = new SelectList(questionTypes.OrderBy(qt => qt.Name), "Id", "Name");
        SectionOptions = new SelectList(sections.OrderBy(s => s.Name), "Id", "Name");
    }
}
