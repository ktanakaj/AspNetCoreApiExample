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
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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

        /// <summary>
        /// コンテキストの変更をDBに保存する。
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">保存成功時にAcceptAllChangesを呼び出すか？</param>
        /// <returns>DBの更新件数。</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            // DB保存前に更新日時の更新を行う
            this.TouchChangedEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// コンテキストの変更をDBに保存する。
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">保存成功時にAcceptAllChangesを呼び出すか？</param>
        /// <param name="cancellationToken">処理キャンセル用トークン。</param>
        /// <returns>DBの更新件数。</returns>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            // DB保存前に更新日時の更新を行う
            this.TouchChangedEntities();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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

        /// <summary>
        /// 変更されているエンティティの登録日時/更新日時を更新する。
        /// </summary>
        private void TouchChangedEntities()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is IHasTimestamp && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var now = DateTimeOffset.UtcNow;
            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IHasTimestamp)entity.Entity).CreatedAt = now;
                }

                ((IHasTimestamp)entity.Entity).UpdatedAt = now;
            }
        }

        #endregion
    }
}