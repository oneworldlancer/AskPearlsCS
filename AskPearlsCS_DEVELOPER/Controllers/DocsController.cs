using System.Text.Json;
using AskPearlsCS_DEVELOPER.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_DEVELOPER.Controllers;

public sealed class DocsController : Controller
{
    private readonly IReadOnlyList<DocumentationPage> _pages;
    public DocsController(IWebHostEnvironment environment)
    {
        var file = Path.Combine(environment.WebRootPath, "docs.json");
        _pages = JsonSerializer.Deserialize<List<DocumentationPage>>(
            System.IO.File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<DocumentationPage>();
    }
    [HttpGet("/")]
    public IActionResult Index() => RedirectToAction(nameof(Page), new { slug = "overview" });

    [HttpGet("/docs/{slug}")]
    public IActionResult Page(string slug)
    {
        var page = _pages.FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        if (page is null) return NotFound();
        ViewData["Pages"] = _pages;
        ViewData["Title"] = page.Title;
        return View(page);
    }
}
