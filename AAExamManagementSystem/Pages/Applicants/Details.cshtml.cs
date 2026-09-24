using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Applicants;

public class DetailsModel : PageModel
{
    private readonly IGenericRepository<Applicant> _repository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;

    public DetailsModel(IGenericRepository<Applicant> repository, IMapper mapper, IWebHostEnvironment environment)
    {
        _repository = repository;
        _mapper = mapper;
        _environment = environment;
    }

    public ApplicantDto Applicant { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var applicant = await _repository.GetByIdAsync(id);
        if (applicant is null)
        {
            return NotFound();
        }

        Applicant = _mapper.Map<ApplicantDto>(applicant);
        return Page();
    }

    // Streams the uploaded resume from App_Data (see Pages/Register.cshtml.cs).
    public async Task<IActionResult> OnGetResumeAsync(int id)
    {
        var applicant = await _repository.GetByIdAsync(id);
        if (applicant?.Resume is null)
        {
            return NotFound();
        }

        var path = Path.Combine(_environment.ContentRootPath, "App_Data", "resumes", Path.GetFileName(applicant.Resume));
        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        var contentType = Path.GetExtension(path) switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            _ => "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
        var downloadName = $"{applicant.LastName}_{applicant.FirstName}_Resume{Path.GetExtension(path)}";
        return PhysicalFile(path, contentType, downloadName);
    }
}
