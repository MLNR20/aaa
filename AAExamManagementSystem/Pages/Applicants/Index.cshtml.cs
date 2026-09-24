using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AAExamManagementSystem.Pages.Applicants;

public class IndexModel : PageModel
{
    private readonly IGenericRepository<Applicant> _repository;
    private readonly IMapper _mapper;

    public IndexModel(IGenericRepository<Applicant> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public IList<ApplicantDto> Applicants { get; set; } = new List<ApplicantDto>();

    public async Task OnGetAsync()
    {
        var applicants = await _repository.GetAllAsync();
        Applicants = _mapper.Map<IList<ApplicantDto>>(applicants.OrderByDescending(a => a.DateCreated));
    }
}
