using Microsoft.AspNetCore.Mvc;

namespace AAExamManagementSystem.ViewComponents;

public class StatusPillViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool isActive, string activeText = "Active", string inactiveText = "Inactive")
    {
        return View((isActive, activeText, inactiveText));
    }
}
