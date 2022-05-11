using System.Net;
using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Kona.Pages;

public class UpdatePostsProressModel : PageModel
{
    private readonly KonaUpdateService _context;

    public UpdatePostsProressModel(Kona.KonaUpdateService context)
    {
        _context = context;
    }
    public List<KonaUpdateService.UpdateItem> updateItems { get; set; }


    public IActionResult OnGet()
    {
        _context.Begin();
        updateItems = _context.GetStatus();
        return Page();
    }
}