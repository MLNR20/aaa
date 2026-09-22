using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Courses;

public class DeleteModel : PageModel
{
    private readonly IGenericRepository<Course> _repository;
    private readonly IMapper _mapper;

    public DeleteModel(IGenericRepository<Course> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public CourseDto Course { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        Course = _mapper.Map<CourseDto>(course);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        _repository.Remove(course);
        await _repository.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Course '{course.Name}' deleted successfully.";
        return RedirectToPage("Index");
    }
}
