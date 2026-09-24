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
            .ForMember(dest => dest.SectionName, opt => opt.Ignore());
        CreateMap<QuestionCreateUpdateDto, Question>();

        CreateMap<ApplicationRole, RoleDto>();
    }
}
