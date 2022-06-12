using System.Net;
using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Kona.Pages;

public class UpdateTagsModel : PageModel
{
    private readonly KonaDB db;

    public UpdateTagsModel(Kona.KonaDB db)
    {
        this.db = db;
    }
    [BindProperty]
    public List<IFormFile> files { get; set; }


    public async Task<IActionResult> OnPost()
    {
        if (null == files || files.Count == 0)
            return Page();

        foreach (var file in files)
        {
            try
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                var json = reader.ReadToEnd();
                var tags = JsonConvert.DeserializeObject<List<DataUtils.tag>>(json);
                if (null == tags)
                    continue;
                DataUtils.AddOrUpdateTags(tags, db);
            }
            catch (Exception e)
            {
                Console.WriteLine("File read error");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }
        }
        return Redirect("/Posts");
    }
}