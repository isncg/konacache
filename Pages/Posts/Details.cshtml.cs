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
    public class DetailsModel : PageModel
    {
        private readonly Kona.KonaDB _context;

        public DetailsModel(Kona.KonaDB context)
        {
            _context = context;
        }

        public Post Post { get; set; }
        public List<Tuple<TagType, List<Tag>>> Tags { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post = await _context.Posts.FirstOrDefaultAsync(m => m.ID == id);

            if (Post == null)
            {
                return NotFound();
            }

            // get tags
            Dictionary<TagType, List<Tag>> tagdict = new Dictionary<TagType, List<Tag>>();
            var tags = _context.PostTags.Where(pt=>pt.Post == Post).Select(pt=>pt.Tag);
            var rawTags =  _context.PostRawTags.Where(pt=>pt.Post == Post).Select(pt=>pt.RawTag);
            var tagNames = _context.PostTags.Where(pt => pt.PostID == Post.ID).Select(pt => pt.Tag.Name);//Post.Tags.Select(t => t.Name).ToList();
            // var tagsRaw = await _context.PostRawTags.Where(pt=>pt.Post == Post).Select(pt=>pt.Tag).ToListAsync();
            // var tagRawNames = tagsRaw.Select(e=>e.Name);

            foreach (var t in tags)
            {
                if (tagdict.TryGetValue(t.Type, out var list))
                    list.Add(t);
                else
                    tagdict[t.Type] = new List<Tag> { t };
            }

            // if (tagRawNames.Count() > tagNames.Count())
            // {
            foreach (var rawTag in rawTags)
            {
                if (!tagNames.Contains(rawTag.Name))
                {
                    if (tagdict.TryGetValue(TagType.Unknown, out var list))
                        list.Add(new Tag { Name = rawTag.Name, Type = TagType.Unknown });
                    else
                        tagdict[TagType.Unknown] = new List<Tag> { new Tag { Name = rawTag.Name, Type = TagType.Unknown } };
                }
            }
            //}
            Tags = new List<Tuple<TagType, List<Tag>>>();
            foreach (var kv in tagdict)
            {
                Tags.Add(new(kv.Key, kv.Value));
            }

            return Page();
        }
    }
}
