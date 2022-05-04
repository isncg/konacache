using System.Net;
using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kona.Pages;

public class UpdatePostsProressModel : PageModel
{
    private readonly PostUpdate _context;

    public UpdatePostsProressModel(Kona.PostUpdate context)
    {
        _context = context;
    }
    public List<PostUpdate.Item> updateItems { get; set; }


    public IActionResult OnGet()
    {
        _context.Begin();
        updateItems = _context.GetStatus();
        return Page();
    }
}