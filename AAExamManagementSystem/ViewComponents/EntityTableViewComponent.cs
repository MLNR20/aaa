using Microsoft.AspNetCore.Mvc;

namespace AAExamManagementSystem.ViewComponents;

public record EntityTableRow(string Id, string Name, bool? IsActive = null);

public record EntityTableViewModel(
    string TableId,
    string BasePath,
    string EmptyMessage,
    IList<EntityTableRow> Items,
    string Title,
    string? Icon,
    string? AddButtonText,
    string? AddButtonModalTarget,
    string? AddButtonHref,
    bool ShowStatusColumn,
    bool ConfirmDeleteWithSweetAlert);

public class EntityTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string tableId,
        string basePath,
        string emptyMessage,
        IEnumerable<EntityTableRow> items,
        string title,
        string? icon = null,
        string? addButtonText = null,
        string? addButtonModalTarget = null,
        string? addButtonHref = null,
        bool showStatusColumn = false,
        bool confirmDeleteWithSweetAlert = false)
    {
        var model = new EntityTableViewModel(
            tableId,
            basePath,
            emptyMessage,
            items.ToList(),
            title,
            icon,
            addButtonText,
            addButtonModalTarget,
            addButtonHref,
            showStatusColumn,
            confirmDeleteWithSweetAlert);

        return View(model);
    }
}
