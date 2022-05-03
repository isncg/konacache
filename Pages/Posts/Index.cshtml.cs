#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Kona;

namespace kona.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly Kona.KonaContext _context;

        public IndexModel(Kona.KonaContext context)
        {
            _context = context;
        }

        public IList<Post> Post { get; set; }
        public int ViewColumn { get; set; } = 4;
        public List<List<Post>> PostGrid { get; private set; }

        public async Task OnGetAsync()
        {
            Post = await _context.Posts.Select(p => p).Where(p => p.Rating == PostRating.S).OrderByDescending(p=>p.ID).ToListAsync();
            PostGrid = new List<List<Post>>();
            int count = Post.Count;
            List<Post> row = null;
            for (int i = 0; i < count; i++)
            {
                if (i % 4 == 0)
                {
                    if (row != null)
                        PostGrid.Add(row);
                    row = new List<Post>();
                }
                row.Add(Post[i]);
            }
            if (null != row && row.Count > 0)
                PostGrid.Add(row);
        }
    }
}
