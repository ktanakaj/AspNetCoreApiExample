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
        /// <summary>
        /// コンテキストを生成する。
        /// </summary>
        /// <param name="options"></param>
        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// ブログテーブル。
        /// </summary>
        public DbSet<Blog> Blogs { get; set; }
    }
}