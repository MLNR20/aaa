using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Services;

// Question + choice persistence shared by the Questions pages. Choices live in their own
// table joined through QuestionAndChoice, which the generic repository can't handle alone.
public class QuestionService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public QuestionService(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IList<QuestionDto>> GetAllAsync()
    {
        var questions = await QuestionsWithDetails().OrderByDescending(q => q.Id).ToListAsync();
        return questions.Select(ToDto).ToList();
    }

    public async Task<QuestionDto?> GetAsync(int id)
    {
        var question = await QuestionsWithDetails().FirstOrDefaultAsync(q => q.Id == id);
        return question is null ? null : ToDto(question);
    }

    public async Task<SelectList> GetQuestionTypeOptionsAsync() =>
        new(await _db.QuestionTypes.Where(qt => qt.IsActive).OrderBy(qt => qt.Name).ToListAsync(), "Id", "Name");

    public async Task<SelectList> GetSectionOptionsAsync() =>
        new(await _db.Sections.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(), "Id", "Name");

    // Ids of the question types that need a choice list, so the form can show/hide the editor.
    public async Task<IList<int>> GetChoiceTypeIdsAsync()
    {
        var types = await _db.QuestionTypes.ToListAsync();
        return types.Where(qt => QuestionTypeNames.UsesChoices(qt.Name)).Select(qt => qt.Id).ToList();
    }

    // Drops blank choice rows and checks the choice rules for the selected question type.
    // Errors are keyed under `prefix` (e.g. "Question") so they show beside the form fields.
    public async Task ValidateAsync(QuestionCreateUpdateDto dto, ModelStateDictionary modelState, string prefix)
    {
        dto.Choices = dto.Choices.Where(c => !string.IsNullOrWhiteSpace(c.ChoiceText)).ToList();

        var questionType = await _db.QuestionTypes.FindAsync(dto.QuestionTypeId);
        if (questionType is null)
        {
            modelState.AddModelError($"{prefix}.QuestionTypeId", "Select a valid question type.");
            return;
        }
        if (!await _db.Sections.AnyAsync(s => s.Id == dto.SectionId))
        {
            modelState.AddModelError($"{prefix}.SectionId", "Select a valid section.");
        }

        if (!QuestionTypeNames.UsesChoices(questionType.Name))
        {
            dto.Choices.Clear();
            return;
        }

        if (questionType.Name == QuestionTypeNames.TrueOrFalse && dto.Choices.Count != 2)
        {
            modelState.AddModelError($"{prefix}.Choices", "A True or False question needs exactly two choices.");
        }
        else if (dto.Choices.Count < 2)
        {
            modelState.AddModelError($"{prefix}.Choices", "Add at least two choices.");
        }

        var correctCount = dto.Choices.Count(c => c.IsCorrect);
        if (correctCount == 0)
        {
            modelState.AddModelError($"{prefix}.Choices", "Mark at least one choice as correct.");
        }
        else if (questionType.Name == QuestionTypeNames.TrueOrFalse && correctCount != 1)
        {
            modelState.AddModelError($"{prefix}.Choices", "A True or False question has exactly one correct choice.");
        }
    }

    public async Task<Question> CreateAsync(QuestionCreateUpdateDto dto)
    {
        var question = _mapper.Map<Question>(dto);
        AddChoices(question, dto.Choices);
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
        return question;
    }

    public async Task<bool> UpdateAsync(int id, QuestionCreateUpdateDto dto)
    {
        var question = await _db.Questions
            .Include(q => q.QuestionAndChoices).ThenInclude(qc => qc.Choice)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (question is null)
        {
            return false;
        }

        question.QuestionTypeId = dto.QuestionTypeId;
        question.SectionId = dto.SectionId;
        question.QuestionTitle = dto.QuestionTitle;
        question.Image = dto.Image;
        question.Score = dto.Score;
        question.IsUpToEvaluation = dto.IsUpToEvaluation;
        question.IsActive = dto.IsActive;
        question.DateUpdated = DateTime.UtcNow;

        // Answers reference the question, not individual choices, so replacing them is safe.
        _db.Choices.RemoveRange(question.QuestionAndChoices.Select(qc => qc.Choice));
        question.QuestionAndChoices.Clear();
        AddChoices(question, dto.Choices);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Found, string? Error)> DeleteAsync(int id)
    {
        var question = await _db.Questions
            .Include(q => q.QuestionAndChoices).ThenInclude(qc => qc.Choice)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (question is null)
        {
            return (false, null);
        }

        if (await _db.Answers.AnyAsync(a => a.QuestionId == id))
        {
            return (true, "This question already has applicant answers. Deactivate it instead of deleting.");
        }

        _db.Choices.RemoveRange(question.QuestionAndChoices.Select(qc => qc.Choice));
        _db.Questions.Remove(question);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    private IQueryable<Question> QuestionsWithDetails() =>
        _db.Questions
            .AsNoTracking()
            .Include(q => q.QuestionType)
            .Include(q => q.Section)
            .Include(q => q.QuestionAndChoices).ThenInclude(qc => qc.Choice);

    private QuestionDto ToDto(Question question)
    {
        var dto = _mapper.Map<QuestionDto>(question);
        dto.QuestionTypeName = question.QuestionType?.Name ?? "—";
        dto.SectionName = question.Section?.Name ?? "—";
        dto.Choices = question.QuestionAndChoices
            .OrderBy(qc => qc.Id)
            .Select(qc => _mapper.Map<ChoiceDto>(qc.Choice))
            .ToList();
        return dto;
    }

    private static void AddChoices(Question question, IEnumerable<ChoiceDto> choices)
    {
        foreach (var choice in choices)
        {
            question.QuestionAndChoices.Add(new QuestionAndChoice
            {
                Choice = new Choice { ChoiceText = choice.ChoiceText.Trim(), IsCorrect = choice.IsCorrect }
            });
        }
    }
}
