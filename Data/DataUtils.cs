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
                !string.IsNullOrWhiteSpace(sample_url);
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

    public static void AddOrUpdateTags(List<tag> jsonTags, KonaDB context)
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

    public static void AddMissingRawTags(IEnumerable<string> tags, KonaDB context)
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

    public static void AddOrUpdatePost(post jsonPost, KonaDB context)
    {
        var post = jsonPost.ToPost();
        var rawTagNames = (post.TagString?.Split(' ') ?? new string[0]).ToHashSet();
        var curTags = context.PostTags.Where(e => e.PostID == post.ID);
        foreach (var tag in curTags)
            context.Entry(tag).State = EntityState.Deleted;
        context.SaveChanges();
        var curRawTags = context.PostRawTags.Where(e => e.PostID == post.ID);
        foreach (var tag in curRawTags)
            context.Entry(tag).State = EntityState.Deleted;
        context.SaveChanges();
        
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

        context.SaveChanges();
    }

    public class TagDownload
    {
        public string tag;
        public virtual string ordertag=>"order:score";
        public virtual string ordername=>"score";
        public string GetURL(int page = 0)
        {
            return string.Format("https://konachan.net/post.json?tags={0}%20{1}&limit=100&page={2}", tag, ordertag, page);
        }
        public string GetName(int page = 0)
        {
            return string.Format("post_{0}_100_{1}_page_{2}.json", tag, ordername, page);
        }

        public string WGetCommand(int page = 0)
        {
            return string.Format("wget -O -c {0} \"{1}\" -–no-check-certificate", GetName(0), GetURL(0));
        }
    }

    public static string FormatJson(string json)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }
}