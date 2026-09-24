using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AAExamManagementSystem.Repository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AAExamManagementSystem.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IGenericRepository<Department> _repository;
    private readonly IMapper _mapper;

    public DepartmentsController(IGenericRepository<Department> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
    {
        var departments = await _repository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department is null) return NotFound();
        return Ok(_mapper.Map<DepartmentDto>(department));
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(DepartmentCreateUpdateDto dto)
    {
        var department = _mapper.Map<Department>(dto);
        await _repository.AddAsync(department);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = department.Id, version = "1.0" }, _mapper.Map<DepartmentDto>(department));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DepartmentCreateUpdateDto dto)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department is null) return NotFound();

        department.Name = dto.Name;
        department.IsActive = dto.IsActive;
        department.DateUpdated = DateTime.UtcNow;
        _repository.Update(department);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department is null) return NotFound();

        _repository.Remove(department);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
