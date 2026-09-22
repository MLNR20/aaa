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

        CreateMap<ApplicationRole, RoleDto>();
    }
}
