using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AutoMapper;

namespace AAExamManagementSystem.Models.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Department, DepartmentDto>();
        CreateMap<DepartmentCreateUpdateDto, Department>();

        CreateMap<Course, CourseDto>();
        CreateMap<CourseCreateUpdateDto, Course>();

        CreateMap<Section, SectionDto>();
        CreateMap<SectionCreateUpdateDto, Section>();

        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.QuestionTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.SectionName, opt => opt.Ignore())
            .ForMember(dest => dest.Choices, opt => opt.MapFrom(src =>
                src.QuestionAndChoices.Select(qc => qc.Choice)));
        CreateMap<QuestionCreateUpdateDto, Question>()
            .ForMember(dest => dest.QuestionAndChoices, opt => opt.Ignore());
        CreateMap<Choice, ChoiceDto>();

        CreateMap<Applicant, ApplicantDto>();
        CreateMap<ApplicantRegistrationDto, Applicant>()
            .ForMember(dest => dest.Resume, opt => opt.Ignore());

        CreateMap<ApplicationRole, RoleDto>();
    }
}
