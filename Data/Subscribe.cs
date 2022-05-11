namespace Kona;
public class Subscribe
{
    public int ID { get; set; }
    public string Name { get; set; }
}

public class SubscribeRawTag
{
    public int ID { get; set; }
    public int SubscribeID { get; set; }
    public Subscribe Subscribe { get; set; }
    public int RawTagID { get; set; }
    public RawTag RawTag { get; set; }
}