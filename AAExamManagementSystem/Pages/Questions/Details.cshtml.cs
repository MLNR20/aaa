using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Questions;

public class DetailsModel : PageModel
{
    private readonly IGenericRepository<Question> _repository;
    private readonly IGenericRepository<QuestionType> _questionTypeRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IMapper _mapper;

    public DetailsModel(
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

    public QuestionDto Question { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }

        Question = _mapper.Map<QuestionDto>(question);
        var questionType = await _questionTypeRepository.GetByIdAsync(question.QuestionTypeId);
        var section = await _sectionRepository.GetByIdAsync(question.SectionId);
        Question.QuestionTypeName = questionType?.Name ?? "—";
        Question.SectionName = section?.Name ?? "—";
        return Page();
    }
}
