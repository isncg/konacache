// <auto-generated />
using Kona;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace kona.Migrations
{
    [DbContext(typeof(KonaDB))]
    [Migration("20220511144625_Rename")]
    partial class Rename
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("Kona.Post", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("File")
                        .HasColumnType("TEXT");

                    b.Property<string>("Preview")
                        .HasColumnType("TEXT");

                    b.Property<byte>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Sample")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .HasColumnType("TEXT");

                    b.Property<string>("TagString")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Kona.PostRawTag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PostID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RawTagID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("PostID");

                    b.HasIndex("RawTagID");

                    b.ToTable("PostRawTags");
                });

            modelBuilder.Entity("Kona.PostTag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PostID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("PostID");

                    b.HasIndex("TagID");

                    b.ToTable("PostTags");
                });

            modelBuilder.Entity("Kona.RawTag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("RawTags");
                });

            modelBuilder.Entity("Kona.Subscribe", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Subscribes");
                });

            modelBuilder.Entity("Kona.SubscribeRawTag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("RawTagID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SubscribeID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("RawTagID");

                    b.HasIndex("SubscribeID");

                    b.ToTable("SubscribeRawTags");
                });

            modelBuilder.Entity("Kona.Tag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Kona.PostRawTag", b =>
                {
                    b.HasOne("Kona.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kona.RawTag", "RawTag")
                        .WithMany()
                        .HasForeignKey("RawTagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("RawTag");
                });

            modelBuilder.Entity("Kona.PostTag", b =>
                {
                    b.HasOne("Kona.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kona.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Kona.SubscribeRawTag", b =>
                {
                    b.HasOne("Kona.RawTag", "RawTag")
                        .WithMany()
                        .HasForeignKey("RawTagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kona.Subscribe", "Subscribe")
                        .WithMany()
                        .HasForeignKey("SubscribeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RawTag");

                    b.Navigation("Subscribe");
                });
#pragma warning restore 612, 618
        }
    }
}
