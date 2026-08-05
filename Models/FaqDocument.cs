namespace FerienspassWebApp.Models;

public class FaqDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Question { get; set; } = string.Empty;

    public string HtmlContent { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}