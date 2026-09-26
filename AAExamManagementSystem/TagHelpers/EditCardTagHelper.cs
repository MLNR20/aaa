using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AAExamManagementSystem.TagHelpers;

[HtmlTargetElement("edit-card")]
public class EditCardTagHelper : TagHelper
{
    public string Title { get; set; } = string.Empty;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = await output.GetChildContentAsync();

        output.TagName = "div";
        output.Attributes.SetAttribute("class", "card border rounded-2 p-3");

        output.Content.SetHtmlContent(
            "<div class=\"card-header bg-white border-bottom-0\">" +
            $"<h5 class=\"mb-0 entity-table-title\">{System.Net.WebUtility.HtmlEncode(Title)}</h5>" +
            "</div>" +
            "<div class=\"card-body\">" +
            childContent.GetContent() +
            "</div>");
    }
}
