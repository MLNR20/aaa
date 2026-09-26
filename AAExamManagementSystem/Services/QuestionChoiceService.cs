using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Services;

public class QuestionChoiceService
{
    public const int MinimumChoices = 2;
    public const int MaximumChoices = 4;

    private readonly ApplicationDbContext _dbContext;

    public QuestionChoiceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<int>> GetChoiceTypeIdsAsync()
    {
        var questionTypes = await _dbContext.QuestionTypes.AsNoTracking().ToListAsync();
        return questionTypes.Where(qt => QuestionTypes.RequiresChoices(qt.Name)).Select(qt => qt.Id).ToList();
    }

    public async Task<List<ChoiceDto>> GetChoicesAsync(Guid questionId)
    {
        return await _dbContext.QuestionAndChoices
            .AsNoTracking()
            .Where(qc => qc.QuestionId == questionId)
            .OrderBy(qc => qc.Id)
            .Select(qc => new ChoiceDto
            {
                Id = qc.Choice.Id,
                ChoiceText = qc.Choice.ChoiceText,
                IsCorrect = qc.Choice.IsCorrect,
                IsActive = qc.Choice.IsActive
            })
            .ToListAsync();
    }

    /// <summary>
    /// Drops blank rows, clears choices for types that don't use them, and validates the rest.
    /// Call before checking ModelState.IsValid.
    /// </summary>
    public async Task NormalizeAndValidateAsync(QuestionCreateUpdateDto dto, ModelStateDictionary modelState, string prefix)
    {
        var choicesKey = string.IsNullOrEmpty(prefix) ? "Choices" : $"{prefix}.Choices";
        foreach (var key in modelState.Keys.Where(k => k.StartsWith(choicesKey, StringComparison.Ordinal)).ToList())
        {
            modelState.Remove(key);
        }

        dto.Choices = dto.Choices
            .Where(c => !string.IsNullOrWhiteSpace(c.ChoiceText))
            .Select(c => { c.ChoiceText = c.ChoiceText.Trim(); return c; })
            .ToList();

        var choiceTypeIds = await GetChoiceTypeIdsAsync();
        if (!choiceTypeIds.Contains(dto.QuestionTypeId))
        {
            dto.Choices.Clear();
            return;
        }

        if (dto.Choices.Count < MinimumChoices)
        {
            modelState.AddModelError(choicesKey, $"Multiple choice questions need at least {MinimumChoices} choices.");
        }
        else if (dto.Choices.Count > MaximumChoices)
        {
            modelState.AddModelError(choicesKey, $"Multiple choice questions can have at most {MaximumChoices} choices.");
        }
        else if (dto.Choices.Count(c => c.IsCorrect) != 1)
        {
            modelState.AddModelError(choicesKey, "Select exactly one choice as the correct answer.");
        }
        else if (dto.Choices.Any(c => c.ChoiceText.Length > 500))
        {
            modelState.AddModelError(choicesKey, "Choice text cannot exceed 500 characters.");
        }
    }

    /// <summary>
    /// Replaces the question's choices with the given list. Does not call SaveChanges.
    /// </summary>
    public async Task ReplaceChoicesAsync(Question question, IEnumerable<ChoiceCreateUpdateDto> choices)
    {
        await RemoveChoicesAsync(question.Id);

        foreach (var dto in choices)
        {
            var choice = new Choice
            {
                ChoiceText = dto.ChoiceText,
                IsCorrect = dto.IsCorrect,
                IsActive = true
            };
            _dbContext.QuestionAndChoices.Add(new QuestionAndChoice { Question = question, Choice = choice });
        }
    }

    /// <summary>
    /// Removes the question's links and their choices. Does not call SaveChanges.
    /// </summary>
    public async Task RemoveChoicesAsync(Guid questionId)
    {
        if (questionId == Guid.Empty) return;

        var links = await _dbContext.QuestionAndChoices
            .Include(qc => qc.Choice)
            .Where(qc => qc.QuestionId == questionId)
            .ToListAsync();

        _dbContext.Choices.RemoveRange(links.Select(l => l.Choice));
        _dbContext.QuestionAndChoices.RemoveRange(links);
    }
}
