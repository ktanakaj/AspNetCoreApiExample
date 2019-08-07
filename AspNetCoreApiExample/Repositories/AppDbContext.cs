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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// アプリケーションDBコンテキストクラス。
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>, IUnitOfWork
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

        #region 公開メソッド

        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        /// <returns>トランザクション。</returns>
        public IDbContextTransaction BeginTransaction()
        {
            return this.Database.BeginTransaction();
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// モデル構築時に呼ばれる処理。
        /// </summary>
        /// <param name="modelBuilder">モデルビルダー。</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ASP.NET Core Identityが自動生成するEntityがMySQLのutf8mb4の
            // 文字数制限でエラーになるので、定義を上書きする。
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(m => m.Email).HasMaxLength(191);
                entity.Property(m => m.NormalizedEmail).HasMaxLength(191);
                entity.Property(m => m.NormalizedUserName).HasMaxLength(191);
                entity.Property(m => m.UserName).HasMaxLength(191);
            });
            modelBuilder.Entity<IdentityRole<int>>(entity =>
            {
                entity.Property(m => m.Name).HasMaxLength(191);
                entity.Property(m => m.NormalizedName).HasMaxLength(191);
            });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(191);
                entity.Property(m => m.ProviderKey).HasMaxLength(191);
            });
            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(191);
                entity.Property(m => m.Name).HasMaxLength(191);
            });
        }

        #endregion
    }
}