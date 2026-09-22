using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Departments;

public class DeleteModel : PageModel
{
    private readonly IGenericRepository<Department> _repository;
    private readonly IMapper _mapper;

    public DeleteModel(IGenericRepository<Department> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public DepartmentDto Department { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department is null)
        {
            return NotFound();
        }

        Department = _mapper.Map<DepartmentDto>(department);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department is null)
        {
            return NotFound();
        }

        _repository.Remove(department);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Department '{department.Name}' deleted successfully.";
        return RedirectToPage("Index");
    }
}
