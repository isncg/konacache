using System.Net;
using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Kona.Pages;

public class UpdatePostsProressModel : PageModel
{
    private readonly KonaUpdateService updateService;

    public UpdatePostsProressModel(Kona.KonaUpdateService updateService)
    {
        this.updateService = updateService;
    }
    public List<KonaUpdateService.UpdateItem> updateItems { get; set; }


    public IActionResult OnGet()
    {
        updateService.Begin();
        updateItems = updateService.GetStatus();
        return Page();
    }
}