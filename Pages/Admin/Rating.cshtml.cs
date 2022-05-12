using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kona.Pages;

class RatingPageModel: PageModel
{
    public bool s;
    public bool q;
    public bool e;

    RatingFilterService filterService;
    public RatingPageModel(RatingFilterService filterService)
    {
        this.filterService = filterService;
    }

    public async Task<IActionResult> OnGetAsync(string? s, string? q, string? e, string? confirm)
    {
        if (confirm == "1")
        {
            filterService.SetFilter(s == "on", q == "on", e == "on");
        }
        filterService.GetFilter(out this.s, out this.q, out this.e);
        return Page();
    }
}