using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kona.Pages;

public class SubscribeCreateMode : PageModel
{
    KonaContext db;
    public SubscribeCreateMode(KonaContext db)
    {
        this.db = db;
    }

    public string error;
    public string inputName;
    public string inputTags;
    public List<RawTag> rawTags;

    public async Task<IActionResult> OnGetAsync(string name, string tags, int confirm)
    {
        var tagNames = tags?.Split(' ') ?? new string[0];
        HashSet<int> rawTagIDs = new HashSet<int>();
        rawTags = new List<RawTag>();
        foreach (var tagName in tagNames)
        {
            var tag = await db.RawTags.Where(e => e.Name == tagName).FirstOrDefaultAsync();
            if (null == tag)
                continue;
            if(rawTagIDs.Contains(tag.ID))
                continue;
            rawTagIDs.Add(tag.ID);
            rawTags.Add(tag);
        }
        if (rawTags.Count == 0)
        {
            error = "No tags";
            return Page();
        }
        error = string.Empty;
        if(confirm == 0)
        {
            return Page();
        }
        Subscribe subscribe = new Subscribe{Name = name};
        db.Subscribes.Add(subscribe);
        db.SaveChanges();
        foreach(var tag in rawTags)
        {
            db.SubscribeRawTags.Add(new SubscribeRawTag{SubscribeID = subscribe.ID, RawTagID = tag.ID});
        }
        db.SaveChanges();
        return Page();
    }
}