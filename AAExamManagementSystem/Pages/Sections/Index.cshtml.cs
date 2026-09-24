using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Pages.Sections;

public class IndexModel : PageModel
{
    private readonly IGenericRepository<Section> _repository;
    private readonly IMapper _mapper;

    public IndexModel(IGenericRepository<Section> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public IList<SectionDto> Sections { get; set; } = new List<SectionDto>();

    [BindProperty]
    public SectionCreateUpdateDto NewSection { get; set; } = new();

    public bool ShowCreateModal { get; set; }

    public async Task OnGetAsync()
    {
        await LoadSectionsAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSectionsAsync();
            ShowCreateModal = true;
            return Page();
        }

        var section = _mapper.Map<Section>(NewSection);
        await _repository.AddAsync(section);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Section '{section.Name}' created successfully.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var section = await _repository.GetByIdAsync(id);
        if (section is null)
        {
            return NotFound();
        }

        _repository.Remove(section);
        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Questions reference sections with ON DELETE RESTRICT.
            TempData["ErrorMessage"] = $"Section '{section.Name}' can't be deleted because it still has questions. Move or delete those questions first.";
            return RedirectToPage("Index");
        }

        TempData["SuccessMessage"] = $"Section '{section.Name}' deleted successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadSectionsAsync()
    {
        var sections = await _repository.GetAllAsync();
        Sections = _mapper.Map<IList<SectionDto>>(sections.OrderBy(s => s.Name));
    }
}
