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
    private readonly IGenericRepository<QuestionType> _questionTypeRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IMapper _mapper;

    public QuestionsController(
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
    {
        var questions = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IList<QuestionDto>>(questions);
        await PopulateLookupsAsync(dtos);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuestionDto>> GetById(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null) return NotFound();

        var dto = _mapper.Map<QuestionDto>(question);
        await PopulateLookupsAsync(new[] { dto });
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(QuestionCreateUpdateDto dto)
    {
        var question = _mapper.Map<Question>(dto);
        await _repository.AddAsync(question);
        await _repository.SaveChangesAsync();

        var result = _mapper.Map<QuestionDto>(question);
        await PopulateLookupsAsync(new[] { result });
        return CreatedAtAction(nameof(GetById), new { id = question.Id, version = "1.0" }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, QuestionCreateUpdateDto dto)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null) return NotFound();

        question.QuestionTypeId = dto.QuestionTypeId;
        question.SectionId = dto.SectionId;
        question.QuestionTitle = dto.QuestionTitle;
        question.Image = dto.Image;
        question.Score = dto.Score;
        question.IsUpToEvaluation = dto.IsUpToEvaluation;
        question.IsActive = dto.IsActive;
        question.DateUpdated = DateTime.UtcNow;
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

    private async Task PopulateLookupsAsync(IEnumerable<QuestionDto> dtos)
    {
        var questionTypes = await _questionTypeRepository.GetAllAsync();
        var sections = await _sectionRepository.GetAllAsync();
        var questionTypeNames = questionTypes.ToDictionary(qt => qt.Id, qt => qt.Name);
        var sectionNames = sections.ToDictionary(s => s.Id, s => s.Name);
        foreach (var dto in dtos)
        {
            dto.QuestionTypeName = questionTypeNames.GetValueOrDefault(dto.QuestionTypeId, string.Empty);
            dto.SectionName = sectionNames.GetValueOrDefault(dto.SectionId, string.Empty);
        }
    }
}
