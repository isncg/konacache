#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kona;
using Microsoft.EntityFrameworkCore;

namespace Kona
{
    public class KonaDB : DbContext
    {
        public KonaDB(DbContextOptions<KonaDB> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RawTag> RawTags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PostRawTag> PostRawTags { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<SubscribeRawTag> SubscribeRawTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().HasMany(p => p.Tags).WithMany(t => t.Posts).UsingEntity<PostTag>();
            modelBuilder.Entity<Post>().HasMany(p => p.RawTags).WithMany(t => t.Posts).UsingEntity<PostRawTag>();
        }
    }


    public enum PostRating : byte
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
        public string? Source { get; set; }
        public string? TagString { get; set; }
        public PostRating Rating { get; set; }
        public List<Tag> Tags { get; set; }
        public List<RawTag> RawTags { get; set; }
    }

    public class PostTag
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public Post Post { get; set; }
        public int TagID { get; set; }
        public Tag Tag { get; set; }
    }

    public class PostRawTag
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public Post Post { get; set; }
        public int RawTagID { get; set; }
        public RawTag RawTag { get; set; }
    }


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
        public List<Post> Posts { get; set; }
    }

    public class RawTag
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }

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
}
