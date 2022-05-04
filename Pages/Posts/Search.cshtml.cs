#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Kona;
using Microsoft.EntityFrameworkCore;

namespace kona.Pages.Posts;

public class SearchModel : PageModel
{
    private readonly Kona.KonaContext _context;

    public SearchModel(Kona.KonaContext context)
    {
        _context = context;
    }
    [BindProperty(SupportsGet = true)]
    public String tags { get; set; }
    [BindProperty(SupportsGet = true)]
    public bool redirect { get; set; }

    //public IList<Post> Post { get; set; }
    public int ViewColumn { get; set; } = 4;
    public List<List<Post>> PostGrid { get; private set; }
    public string downloads {get; private set;}

    public async Task<IActionResult> OnGetAsync()
    {
        var tagNames = tags?.Split(' ') ?? new string[] { };
        if (tagNames.Length == 0 && redirect)
        {
            return Redirect("/Posts");
        }
        List<Post> postList = new List<Post>();
        HashSet<int> ids = new HashSet<int>();
        var tagDownloads = new List<DataUtils.TagDownload>();
        foreach (var tagName in tagNames)
        {
            var posts = await _context.PostRawTags.Where(pt => pt.RawTag.Name == tagName).Select(pt => pt.Post).ToListAsync();
            foreach (var p in posts)
            {
                if (ids.Contains(p.ID) || p.Rating != PostRating.S)
                    continue;
                postList.Add(p);
                ids.Add(p.ID);
            }
            tagDownloads.Add(new DataUtils.TagDownload{tag = tagName});
        }
        postList.Sort((a, b) => b.ID - a.ID);
        PostGrid = new List<List<Post>>();
        int count = postList.Count;
        List<Post> row = null;
        for (int i = 0; i < count; i++)
        {
            if (i % 4 == 0)
            {
                if (row != null)
                    PostGrid.Add(row);
                row = new List<Post>();
            }
            row.Add(postList[i]);
        }
        if (null != row && row.Count > 0)
            PostGrid.Add(row);
        downloads = string.Join('\n', tagDownloads.ConvertAll(e=>e.WGetCommand()));
        return Page();
    }
}