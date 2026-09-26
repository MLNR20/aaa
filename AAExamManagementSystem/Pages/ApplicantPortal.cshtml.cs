using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AAExamManagementSystem.Pages;

public class ApplicantPortalModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public ApplicantPortalModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    public ApplicantDto? Applicant { get; set; }
    public int AttemptCount { get; set; }

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        var applicant = await _context.Applicants
            .Include(a => a.Attempts)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (applicant is null)
        {
            return;
        }

        Applicant = _mapper.Map<ApplicantDto>(applicant);
        AttemptCount = applicant.Attempts.Count;
    }
}
