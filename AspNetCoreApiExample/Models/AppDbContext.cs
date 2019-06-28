// ================================================================================================
// <summary>
//      アプリケーションDBコンテキストクラスソース</summary>
//
// <copyright file="AppDbContext.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Models
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// アプリケーションDBコンテキストクラス。
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// コンテキストを生成する。
        /// </summary>
        /// <param name="options">オプション。</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// ブログテーブル。
        /// </summary>
        public DbSet<Blog> Blogs { get; set; }

        /// <summary>
        /// ブログ記事テーブル。
        /// </summary>
        public DbSet<Article> Articles { get; set; }
    }
}