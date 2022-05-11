using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kona.Pages;

public class SubscribeIndexModel : PageModel
{
    public class Item
    {
        public Kona.Subscribe subscribe;
        public List<RawTag> tags;
        public List<Post> recentPosts;
    }

    public List<Item> items;
    KonaContext db;

    public SubscribeIndexModel(KonaContext db)
    {
        this.db = db;
    }


    public async Task<IActionResult> OnGetAsync()
    {
        items = (await db.Subscribes.ToListAsync()).ConvertAll(p => new Item { subscribe = p, tags = new List<RawTag>() });
        items?.ForEach(async item =>
        {
            var query = db.SubscribeRawTags.Where(e => e.SubscribeID == item.subscribe.ID).Select(e => e.RawTag);
            item.tags = await query.ToListAsync();
            List<IQueryable<Post>> queryableList = new List<IQueryable<Post>>();
            IQueryable<Post> joinedResult = null;
            foreach (var rt in item.tags)
            {
                queryableList.Add(db.PostRawTags.Where(e => e.RawTagID == rt.ID && e.Post.Rating == PostRating.S).Select(e => e.Post));
            }
            if (queryableList.Count > 0)
            {
                joinedResult = queryableList[0];
                for (int i = 1; i < queryableList.Count; i++)
                {
                    joinedResult = Queryable.Join(joinedResult, queryableList[i], e => e.ID, e => e.ID, (o, i) => o);
                }
            }
            if(null!=joinedResult)
                item.recentPosts = await joinedResult.OrderByDescending(e=>e.ID).Take(3).ToListAsync();
            //int count = joinedResult?.Count()??0;
        });

        return Page();
    }
}