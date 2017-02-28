using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Rikka.Tsab2.Database.Context;
using Rikka.Tsab2.Database.Context.Entities;

namespace Rikka.Tsab2.Database.Generator.Migrations
{
    [DbContext(typeof(TsabContext))]
    partial class TsabContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChatId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("State");

                    b.Property<string>("StateParams");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.SearchEngine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Additional");

                    b.Property<int>("ChatId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("SearchEngine");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkAuth", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChatId");

                    b.Property<string>("Code");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("ExpiredAt");

                    b.Property<string>("Token");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.ToTable("VkAuth");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkPhoto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Content")
                        .HasColumnType("image");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("EntryId");

                    b.Property<string>("Url");

                    b.Property<Guid>("WallId");

                    b.HasKey("Id");

                    b.HasIndex("EntryId");

                    b.HasIndex("WallId");

                    b.ToTable("VkPhoto");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkWall", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("LastUpdate");

                    b.Property<string>("Name");

                    b.Property<int>("Type");

                    b.Property<int?>("UploadAlbum");

                    b.Property<string>("Url");

                    b.Property<int>("VkWallId");

                    b.HasKey("Id");

                    b.ToTable("VkWall");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkWallEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Text");

                    b.Property<string>("Url");

                    b.Property<Guid>("WallId");

                    b.HasKey("Id");

                    b.HasIndex("WallId");

                    b.ToTable("VkWallEntry");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkAuth", b =>
                {
                    b.HasOne("Rikka.Tsab2.Database.Context.Entities.Chat", "Chat")
                        .WithMany("VkAuths")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkPhoto", b =>
                {
                    b.HasOne("Rikka.Tsab2.Database.Context.Entities.VkWallEntry", "Entry")
                        .WithMany("Photos")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Rikka.Tsab2.Database.Context.Entities.VkWall", "Wall")
                        .WithMany("Photos")
                        .HasForeignKey("WallId");
                });

            modelBuilder.Entity("Rikka.Tsab2.Database.Context.Entities.VkWallEntry", b =>
                {
                    b.HasOne("Rikka.Tsab2.Database.Context.Entities.VkWall", "Wall")
                        .WithMany()
                        .HasForeignKey("WallId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
