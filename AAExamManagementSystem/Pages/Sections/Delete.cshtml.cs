using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Sections;

public class DeleteModel : PageModel
{
    private readonly IGenericRepository<Section> _repository;
    private readonly IMapper _mapper;

    public DeleteModel(IGenericRepository<Section> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public SectionDto Section { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var section = await _repository.GetByIdAsync(id);
        if (section is null)
        {
            return NotFound();
        }

        Section = _mapper.Map<SectionDto>(section);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var section = await _repository.GetByIdAsync(id);
        if (section is null)
        {
            return NotFound();
        }

        _repository.Remove(section);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Section '{section.Name}' deleted successfully.";
        return RedirectToPage("Index");
    }
}
