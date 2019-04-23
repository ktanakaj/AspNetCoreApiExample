// ================================================================================================
// <summary>
//      ブログ記事コンテキストクラスソース</summary>
//
// <copyright file="ArticleContext.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Models
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// ブログ記事コンテキストクラス。
    /// </summary>
    public class ArticleContext : DbContext
    {
        /// <summary>
        /// コンテキストを生成する。
        /// </summary>
        /// <param name="options"></param>
        public ArticleContext(DbContextOptions<ArticleContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// ブログ記事テーブル。
        /// </summary>
        public DbSet<Article> Articles { get; set; }
    }
}