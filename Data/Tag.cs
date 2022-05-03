using Kona;

namespace kona;
public enum TagType
{
    General = 0,
    Artist = 1,
    Copyright = 3,
    Character = 4,
    Style = 5,
    Circle = 6,
    Unknown = 7,
}

public class Tag
{
    public int ID { get; set; }
    public TagType Type { get; set; }
    public string Name { get; set; }
    public List<Post> Posts {get;set;}
}

public class RawTag
{
    public int ID { get; set; }
    public string Name { get; set; }
    public List<Post> Posts {get;set;}
}