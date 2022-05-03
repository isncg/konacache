using kona;

namespace Kona;
public enum PostRating: byte
{
    S = 0,
    Q = 1,
    E = 2,
}
public class Post
{
    public int ID { get; set; }
    public string? Preview { get; set; }
    public string? Sample { get; set; }
    public string? File { get; set; }
    public string? Source {get; set; }
    public string? TagString {get; set;}
    public PostRating Rating { get; set; }
    public List<Tag> Tags {get; set;}
    public List<RawTag> RawTags {get; set;}
}

public class PostTag
{
    public int ID {get; set;}
    public int PostID {get; set;}
    public Post Post {get; set;}
    public int TagID {get; set;}
    public Tag Tag {get; set;}
}

public class PostRawTag
{
    public int ID {get; set;}
    public int PostID {get; set;}
    public Post Post {get; set;}
    public int RawTagID {get; set;}
    public RawTag RawTag {get; set;}
}