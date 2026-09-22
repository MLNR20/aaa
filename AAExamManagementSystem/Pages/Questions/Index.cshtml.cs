using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AAExamManagementSystem.Pages.Questions;

public class IndexModel : PageModel
{
    private readonly IGenericRepository<Question> _repository;
    private readonly IGenericRepository<Exam> _examRepository;
    private readonly IMapper _mapper;

    public IndexModel(IGenericRepository<Question> repository, IGenericRepository<Exam> examRepository, IMapper mapper)
    {
        _repository = repository;
        _examRepository = examRepository;
        _mapper = mapper;
    }

    public IList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();

    public SelectList ExamOptions { get; set; } = new(new List<Exam>(), "Id", "Title");

    [BindProperty]
    public QuestionCreateUpdateDto NewQuestion { get; set; } = new();

    public bool ShowCreateModal { get; set; }

    public async Task OnGetAsync()
    {
        await LoadQuestionsAsync();
        await LoadExamOptionsAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadQuestionsAsync();
            await LoadExamOptionsAsync();
            ShowCreateModal = true;
            return Page();
        }

        var question = _mapper.Map<Question>(NewQuestion);
        await _repository.AddAsync(question);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = "Question created successfully.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
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

    private async Task LoadQuestionsAsync()
    {
        var questions = await _repository.GetAllAsync();
        var exams = await _examRepository.GetAllAsync();
        var examTitles = exams.ToDictionary(e => e.Id, e => e.Title);

        Questions = _mapper.Map<IList<QuestionDto>>(questions.OrderByDescending(q => q.Id));
        foreach (var dto in Questions)
        {
            dto.ExamTitle = examTitles.GetValueOrDefault(dto.ExamId, "—");
        }
    }

    private async Task LoadExamOptionsAsync()
    {
        var exams = await _examRepository.GetAllAsync();
        ExamOptions = new SelectList(exams.OrderBy(e => e.Title), "Id", "Title");
    }
}
