using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text;
using System.Text.RegularExpressions;
using kona;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace Kona;
public static class DataUtils
{
    public enum rating
    {
        s = 0,
        q = 1,
        e = 2
    }
    public class post
    {
        public int id;
        public string tags;
        public string file_url;
        public string preview_url;
        public string sample_url;
        public string source;
        public rating rating;

        public bool IsValid
        {
            get
            {
                return
                !string.IsNullOrWhiteSpace(file_url) &&
                !string.IsNullOrWhiteSpace(preview_url) &&
                !string.IsNullOrWhiteSpace(sample_url) &&
                !string.IsNullOrWhiteSpace(source);
            }
        }

        public Post ToPost()
        {
            return new Post
            {
                ID = id,
                TagString = tags,
                File = file_url,
                Preview = preview_url,
                Sample = sample_url,
                Source = source,
                Rating = (PostRating)rating,
                Tags = new List<Tag>(),
                RawTags = new List<RawTag>()
            };
        }

    }

    public class tag
    {
        public int id;
        public string name;
        public int count;
        public TagType type;

        public Tag ToTag()
        {
            return new Tag { ID = id, Name = name, Type = type };
        }
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(name) && count > 0 && (int)type < (int)TagType.Unknown;
            }
        }
    }

    public static void AddOrUpdateTags(List<tag> jsonTags, KonaContext context)
    {
        var tagList = jsonTags.ConvertAll(t => t.ToTag());
        var ids = context.Tags.Select(e => e.ID);
        Dictionary<string, List<int>> posttags = new Dictionary<string, List<int>>();
        foreach (var tag in tagList)
        {
            if (ids.Contains(tag.ID))
            {
                context.Entry(tag).State = EntityState.Modified;
            }
            else
            {
                context.Entry(tag).State = EntityState.Added;
                posttags[tag.Name] = context.PostRawTags.Where(e => e.RawTag.Name == tag.Name).Select(e => e.PostID).ToList();
            }
        }
        context.SaveChanges();
        List<PostTag> toAdd = new List<PostTag>();
        foreach (var kv in posttags)
        {
            var tag = context.Tags.Where(e => e.Name == kv.Key).FirstOrDefault();
            if (null == tag)
                continue;
            foreach (var pid in kv.Value)
            {
                toAdd.Add(new PostTag { PostID = pid, TagID = tag.ID });
            }
        }
        context.PostTags.AddRange(toAdd);
        context.SaveChanges();
    }

    public static void AddMissingRawTags(IEnumerable<string> tags, KonaContext context)
    {
        var curRawTags = context.RawTags.Select(e => e.Name);
        List<RawTag> rawTagsToAdd = new List<RawTag>();
        foreach (var name in tags)
        {
            if (!curRawTags.Contains(name))
                rawTagsToAdd.Add(new RawTag { Name = name });
        }
        context.RawTags.AddRange(rawTagsToAdd);
        context.SaveChanges();
    }

    public static async Task AddOrUpdatePost(post jsonPost, KonaContext context)
    {
        var post = jsonPost.ToPost();
        var rawTagNames = (post.TagString?.Split(' ') ?? new string[0]).ToHashSet();
        AddMissingRawTags(rawTagNames, context);

        foreach (var t in rawTagNames)
        {
            var rawTag = context.RawTags.Where(e => e.Name == t).FirstOrDefault();
            if (null != rawTag)
                post.RawTags.Add(rawTag);
            var tag = context.Tags.Where(e => e.Name == t).FirstOrDefault();
            if (null != tag)
                post.Tags.Add(tag);
        }
        var isUpdate = context.Posts.Where(e => e.ID == post.ID).Count() > 0;
        if (isUpdate)
        {
            context.Posts.Update(post);
        }
        else
        {
            context.Posts.Add(post);
        }

        await context.SaveChangesAsync();
    }


    public static void LoadSampleData(KonaContext context)
    {
        // var sampleTags = JsonConvert.DeserializeObject<List<tag>>(File.ReadAllText("tag.json"));
        // if (null != sampleTags)
        // {
        //     AddOrUpdateTags(sampleTags, context);
        // }

        // var samplePosts = JsonConvert.DeserializeObject<List<post>>(File.ReadAllText("post.json"));
        // if (null != samplePosts)
        // {
        //     foreach (var p in samplePosts)
        //     {
        //         AddOrUpdatePost(p, context);
        //     }
        // }
    }
}