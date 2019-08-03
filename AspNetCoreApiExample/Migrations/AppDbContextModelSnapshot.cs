﻿// <auto-generated />
using Honememo.AspNetCoreApiExample.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Honememo.AspNetCoreApiExample.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Honememo.AspNetCoreApiExample.Entities.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasMaxLength(65535);

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(191);

                    b.HasKey("Id");

                    b.HasIndex("BlogId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Honememo.AspNetCoreApiExample.Entities.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(191);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("Honememo.AspNetCoreApiExample.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(191);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(191);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Honememo.AspNetCoreApiExample.Entities.Article", b =>
                {
                    b.HasOne("Honememo.AspNetCoreApiExample.Entities.Blog", "Blog")
                        .WithMany("Articles")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Honememo.AspNetCoreApiExample.Entities.Blog", b =>
                {
                    b.HasOne("Honememo.AspNetCoreApiExample.Entities.User", "User")
                        .WithMany("Blogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
