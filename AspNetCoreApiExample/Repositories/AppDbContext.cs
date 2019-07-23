// ================================================================================================
// <summary>
//      アプリケーションDBコンテキストクラスソース</summary>
//
// <copyright file="AppDbContext.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// アプリケーションDBコンテキストクラス。
    /// </summary>
    public class AppDbContext : DbContext, IUnitOfWork
    {
        #region コンストラクタ

        /// <summary>
        /// コンテキストを生成する。
        /// </summary>
        /// <param name="options">オプション。</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ブログテーブル。
        /// </summary>
        public DbSet<Blog> Blogs { get; set; }

        /// <summary>
        /// ブログ記事テーブル。
        /// </summary>
        public DbSet<Article> Articles { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        /// <returns>トランザクション。</returns>
        public IDbContextTransaction BeginTransaction()
        {
            return this.Database.BeginTransaction();
        }

        #endregion
    }
}