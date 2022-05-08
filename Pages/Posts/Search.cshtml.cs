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
    private readonly IConfiguration _configuration;

    public SearchModel(Kona.KonaContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [BindProperty(SupportsGet = true)]
    public String tags { get; set; }
    [BindProperty(SupportsGet = true)]
    public String redirect { get; set; }

    //public IList<Post> Post { get; set; }
    public int ViewColumn { get; set; } = 4;
    public List<List<Post>> PostGrid { get; private set; }

    public PaginatedList<Post> Posts { get; set; }
    public string downloads { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? pageIndex)
    {
        var tagNames = tags?.Split(' ') ?? new string[] { };
        var pageSize = _configuration.GetValue<int>("PostsPerPage", 20);
        PostGrid = new List<List<Post>>();
        if (tagNames.Length == 0)
        {
            if(string.IsNullOrWhiteSpace(redirect))
                return Redirect("/Posts/Index");
            return Redirect(redirect);
        }
        HashSet<int> rawTagIDs = new HashSet<int>();
        foreach (var name in tagNames)
        {
            var rawTag = _context.RawTags.Where(e => e.Name == name).FirstOrDefault();
            if (null != rawTag)
                rawTagIDs.Add(rawTag.ID);
        }
        IQueryable<Post> joinedResult;
        List<IQueryable<Post>> queryableList = new List<IQueryable<Post>>();
        foreach (var rtid in rawTagIDs)
        {
            queryableList.Add(_context.PostRawTags.Where(e => e.RawTagID == rtid && e.Post.Rating == PostRating.S).Select(e => e.Post));
        }
        if (queryableList.Count > 0)
        {
            joinedResult = queryableList[0];
            for (int i = 1; i < queryableList.Count; i++)
            {
                joinedResult = Queryable.Join(joinedResult, queryableList[i], e => e.ID, e => e.ID, (o, i) => o);
            }
            Posts = await PaginatedList<Post>.CreateAsync(joinedResult, pageIndex ?? 1, pageSize: pageSize);
            int count = Posts.Count;
            List<Post> row = null;
            for (int i = 0; i < count; i++)
            {
                if (i % 4 == 0)
                {
                    if (row != null)
                        PostGrid.Add(row);
                    row = new List<Post>();
                }
                row.Add(Posts[i]);
            }
            if (null != row && row.Count > 0)
                PostGrid.Add(row);
        }
        return Page();
    }
}