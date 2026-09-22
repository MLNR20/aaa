using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AAExamManagementSystem.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IGenericRepository<Question> _repository;
    private readonly IGenericRepository<Exam> _examRepository;
    private readonly IMapper _mapper;

    public QuestionsController(IGenericRepository<Question> repository, IGenericRepository<Exam> examRepository, IMapper mapper)
    {
        _repository = repository;
        _examRepository = examRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
    {
        var questions = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IList<QuestionDto>>(questions);
        await PopulateExamTitlesAsync(dtos);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuestionDto>> GetById(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null) return NotFound();

        var dto = _mapper.Map<QuestionDto>(question);
        await PopulateExamTitlesAsync(new[] { dto });
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(QuestionCreateUpdateDto dto)
    {
        var question = _mapper.Map<Question>(dto);
        await _repository.AddAsync(question);
        await _repository.SaveChangesAsync();

        var result = _mapper.Map<QuestionDto>(question);
        await PopulateExamTitlesAsync(new[] { result });
        return CreatedAtAction(nameof(GetById), new { id = question.Id, version = "1.0" }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, QuestionCreateUpdateDto dto)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null) return NotFound();

        question.ExamId = dto.ExamId;
        question.QuestionText = dto.QuestionText;
        question.QuestionType = dto.QuestionType;
        question.OptionA = dto.OptionA;
        question.OptionB = dto.OptionB;
        question.OptionC = dto.OptionC;
        question.OptionD = dto.OptionD;
        question.CorrectAnswer = dto.CorrectAnswer;
        question.Points = dto.Points;
        question.IsActive = dto.IsActive;
        _repository.Update(question);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null) return NotFound();

        _repository.Remove(question);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    private async Task PopulateExamTitlesAsync(IEnumerable<QuestionDto> dtos)
    {
        var exams = await _examRepository.GetAllAsync();
        var examTitles = exams.ToDictionary(e => e.Id, e => e.Title);
        foreach (var dto in dtos)
        {
            dto.ExamTitle = examTitles.GetValueOrDefault(dto.ExamId, string.Empty);
        }
    }
}
