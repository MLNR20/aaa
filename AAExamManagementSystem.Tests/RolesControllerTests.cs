using AAExamManagementSystem.Controllers;
using AAExamManagementSystem.Models.Dtos;
using AAExamManagementSystem.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AAExamManagementSystem.Tests;

public class RolesControllerTests
{
    private static Mock<IRoleStore<ApplicationRole>> CreateStoreMock()
    {
        var store = new Mock<IRoleStore<ApplicationRole>>();

        store.Setup(s => s.GetRoleNameAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationRole r, CancellationToken _) => r.Name);
        store.Setup(s => s.SetNormalizedRoleNameAsync(It.IsAny<ApplicationRole>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        store.Setup(s => s.CreateAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        return store;
    }

    private static RoleManager<ApplicationRole> CreateRoleManager(Mock<IRoleStore<ApplicationRole>> store)
    {
        return new RoleManager<ApplicationRole>(store.Object, null!, null!, null!, null!);
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.CreateMap<ApplicationRole, RoleDto>(),
            Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        return configuration.CreateMapper();
    }

    [Fact]
    public async Task Create_WithDuplicateRoleName_ReturnsBadRequest_AndDoesNotCallStoreCreate()
    {
        var store = CreateStoreMock();
        store.Setup(s => s.FindByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApplicationRole("Admin"));

        var controller = new RolesController(CreateRoleManager(store), CreateMapper());

        var result = await controller.Create(new RoleCreateUpdateDto { Name = "Admin" });

        Assert.IsType<BadRequestObjectResult>(result.Result);
        store.Verify(s => s.CreateAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithUniqueRoleName_CreatesRole()
    {
        var store = CreateStoreMock();
        store.Setup(s => s.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationRole?)null);

        var controller = new RolesController(CreateRoleManager(store), CreateMapper());

        var result = await controller.Create(new RoleCreateUpdateDto { Name = "Grader" });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<RoleDto>(created.Value);
        Assert.Equal("Grader", dto.Name);
        store.Verify(s => s.CreateAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_ChangingToExistingRoleName_ReturnsBadRequest()
    {
        var store = CreateStoreMock();
        var existingRole = new ApplicationRole("Instructor") { Id = "role-1" };
        store.Setup(s => s.FindByIdAsync("role-1", It.IsAny<CancellationToken>())).ReturnsAsync(existingRole);
        store.Setup(s => s.FindByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApplicationRole("Admin"));

        var controller = new RolesController(CreateRoleManager(store), CreateMapper());

        var result = await controller.Update("role-1", new RoleCreateUpdateDto { Name = "Admin" });

        Assert.IsType<BadRequestObjectResult>(result);
        store.Verify(s => s.UpdateAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
