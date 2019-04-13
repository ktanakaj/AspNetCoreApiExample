// ================================================================================================
// <summary>
//      ブログコンテキストクラスソース</summary>
//
// <copyright file="BlogContext.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Models
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// ブログコンテキストクラス。
    /// </summary>
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
    }
}