#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Kona;

namespace kona.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly Kona.KonaDB _context;
        private readonly IConfiguration _configuration;
        private readonly RatingFilterService _filter;

        public IndexModel(Kona.KonaDB context, IConfiguration configuration, RatingFilterService filter)
        {
            _context = context;
            _configuration = configuration;
            _filter = filter;
        }

        //public IList<Post> Post { get; set; }
        public int ViewColumn { get; set; } = 4;
        public List<List<Post>> PostGrid { get; private set; }

        public PaginatedList<Post> Posts { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            var pageSize = _configuration.GetValue<int>("PostsPerPage", 20);
            Posts = await PaginatedList<Post>.CreateAsync(_context.Posts.SelectFilter(_filter).OrderByDescending(p => p.ID), pageIndex ?? 1, pageSize: pageSize);
            // Post = await _context.Posts.Select(p => p).Where(p => p.Rating == PostRating.S).OrderByDescending(p=>p.ID).ToListAsync();
            PostGrid = new List<List<Post>>();
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
    }
}
