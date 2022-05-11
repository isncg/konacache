#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kona;
using Microsoft.EntityFrameworkCore;

namespace Kona
{
    public class KonaContext : DbContext
    {
        public KonaContext (DbContextOptions<KonaContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags {get; set;}
        public DbSet<RawTag> RawTags {get; set;}
        public DbSet<PostTag> PostTags {get; set;}
        public DbSet<PostRawTag> PostRawTags {get; set;}
        public DbSet<Subscribe> Subscribes {get;set;}
        public DbSet<SubscribeRawTag> SubscribeRawTags {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().HasMany(p=>p.Tags).WithMany(t=>t.Posts).UsingEntity<PostTag>();
            modelBuilder.Entity<Post>().HasMany(p=>p.RawTags).WithMany(t=>t.Posts).UsingEntity<PostRawTag>();
        }
    }
}
