namespace AskPearlsCS_DEVELOPER.Models;
public sealed class DocumentationPage
{
    public string Slug { get; set; } = "";
    public string Group { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<DocumentationStep> Steps { get; set; } = new();
    public List<string> Notes { get; set; } = new();
    public string? Code { get; set; }
    public string? Portal { get; set; }
}
public sealed class DocumentationStep
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
}
