using Microsoft.AspNetCore.Mvc;

namespace AAExamManagementSystem.ViewComponents;

public record DetailsField(string Label, string Value);

public record DetailsCardViewModel(
    string Title,
    IList<DetailsField> Fields,
    bool ShowStatus,
    bool IsActive,
    string EditHref,
    string BackHref,
    string ColumnClass);

public class DetailsCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string title,
        IEnumerable<DetailsField> fields,
        string editHref,
        string backHref,
        bool showStatus = false,
        bool isActive = false,
        string columnClass = "col-md-4")
    {
        var model = new DetailsCardViewModel(title, fields.ToList(), showStatus, isActive, editHref, backHref, columnClass);
        return View(model);
    }
}
