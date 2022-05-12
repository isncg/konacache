using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kona.Pages;
public class SubscribeEditVM
{
    public Kona.Subscribe subscribe;
    public List<RawTag> tags;
    public string inputID => subscribe?.ID.ToString() ?? string.Empty;
    public string inputName => subscribe?.Name ?? string.Empty;
    public string inputTags => null == tags ? string.Empty : string.Join(' ', tags.ConvertAll(e => e.Name));
    public static async Task<SubscribeEditVM> GetItem(Kona.Subscribe subscribe, KonaContext db)
    {
        var result = new SubscribeEditVM();
        result.subscribe = subscribe;
        result.tags = await db.SubscribeRawTags.Where(e => e.SubscribeID == subscribe.ID).Select(e => e.RawTag).ToListAsync();
        return result;
    }
}
public class SubscribeEditModel: PageModel
{
    KonaContext db;
    public SubscribeEditVM vm; 
    public string error;
    public SubscribeEditModel(KonaContext db)
    {
        this.db = db;
    }
    
    public async Task<IActionResult> OnGetAsync(int id, string newTags, string newName)
    {
        var subscribe = await db.Subscribes.FirstOrDefaultAsync(e=>e.ID == id);
        if (null == subscribe)
        {
            error = $"Cannot find subscribe with id '{id}'";
            return Page();
        }
        if (!string.IsNullOrWhiteSpace(newTags))
        {
            var tagNames = newTags.Split(' ');
            List<RawTag> rawTags = new List<RawTag>();
            bool isError = false;
            foreach (var tagName in tagNames)
            {
                var rawTag = await db.RawTags.Where(e => e.Name == tagName).FirstOrDefaultAsync();
                if (null == rawTag)
                {
                    isError = true;
                    error += $"Cannot find tag '{tagName}'\n";
                }
                else
                {
                    rawTags.Add(rawTag);
                }
            }
            if (!isError)
            {
                db.RemoveRange(db.SubscribeRawTags.Where(e => e.SubscribeID == id));
                db.SaveChanges();
                foreach (var tag in rawTags)
                    db.SubscribeRawTags.Add(new SubscribeRawTag { SubscribeID = id, RawTagID = tag.ID });
                db.SaveChanges();
            }
        }

        if(!string.IsNullOrWhiteSpace(newName))
        {
            subscribe.Name = newName;
            db.Entry(subscribe).State = EntityState.Modified;
            db.SaveChanges();
        }
        
        vm = await SubscribeEditVM.GetItem(subscribe, db);
        return Page();
    }
}