using System.Net;
using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kona.Pages;

public class UpdatePostsModel : PageModel
{
    private readonly KonaContext _context;

    public UpdatePostsModel(Kona.KonaContext context)
    {
        _context = context;
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
                var rawPosts = JsonConvert.DeserializeObject<List<DataUtils.post>>(json);
                if (null != rawPosts)
                {
                    foreach (var p in rawPosts)
                    {
                        if (p.IsValid)
                            await DataUtils.AddOrUpdatePost(p, _context);
                    }
                }
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


public class FileUpload
{
    public List<IFormFile> FormFiles { get; set; } // convert to list
    public string SuccessMessage { get; set; }
}