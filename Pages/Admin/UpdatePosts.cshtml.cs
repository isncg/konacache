using System.Net;
using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Kona.Pages;

public class UpdatePostsModel : PageModel
{
    private readonly KonaUpdateService _context;

    public UpdatePostsModel(Kona.KonaUpdateService context)
    {
        _context = context;
    }
    [BindProperty]
    public List<IFormFile> files { get; set; }

    public IActionResult OnPost()
    {
        if (null == files || files.Count == 0)
            return Page();
        foreach (var file in files)
        {
            try
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                var json = reader.ReadToEnd();
                _context.Add(KonaUpdateService.UpdateItemType.Posts, file.FileName, json);
            }
            catch (Exception e)
            {
                Console.WriteLine("File read error");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }
        }
        return RedirectToPage("./UpdatePostsProgress");
    }
}