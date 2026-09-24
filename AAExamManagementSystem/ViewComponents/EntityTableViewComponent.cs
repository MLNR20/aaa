using Microsoft.AspNetCore.Mvc;

namespace AAExamManagementSystem.ViewComponents;

public record EntityTableRow(string Id, string Name, bool? IsActive = null, DateTime? DateCreated = null, DateTime? DateUpdated = null);

public record EntityTableViewModel(
    string TableId,
    string BasePath,
    string EmptyMessage,
    IList<EntityTableRow> Items,
    string Title,
    string? Icon,
    string? TitleClass,
    string? AddButtonText,
    string? AddButtonModalTarget,
    string? AddButtonHref,
    bool ShowStatusColumn,
    bool ShowDateCreatedColumn,
    bool ShowDateUpdatedColumn,
    bool ConfirmDeleteWithSweetAlert,
    bool ShowRowNumberColumn);

public class EntityTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string tableId,
        string basePath,
        string emptyMessage,
        IEnumerable<EntityTableRow> items,
        string title,
        string? icon = null,
        string? titleClass = null,
        string? addButtonText = null,
        string? addButtonModalTarget = null,
        string? addButtonHref = null,
        bool showStatusColumn = false,
        bool showDateCreatedColumn = false,
        bool showDateUpdatedColumn = false,
        bool confirmDeleteWithSweetAlert = false,
        bool showRowNumberColumn = false)
    {
        var model = new EntityTableViewModel(
            tableId,
            basePath,
            emptyMessage,
            items.ToList(),
            title,
            icon,
            titleClass,
            addButtonText,
            addButtonModalTarget,
            addButtonHref,
            showStatusColumn,
            showDateCreatedColumn,
            showDateUpdatedColumn,
            confirmDeleteWithSweetAlert,
            showRowNumberColumn);

        return View(model);
    }
}
