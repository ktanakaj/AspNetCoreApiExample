// ================================================================================================
// <summary>
//      アプリケーションDBコンテキストクラスソース</summary>
//
// <copyright file="AppDbContext.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Reflection;
using Honememo.AspNetCoreApiExample.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Honememo.AspNetCoreApiExample.Repositories
{
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
        public DbSet<Blog> Blogs => this.Set<Blog>();

        /// <summary>
        /// ブログ記事テーブル。
        /// </summary>
        public DbSet<Article> Articles => this.Set<Article>();

        /// <summary>
        /// ブログ記事のタグテーブル。
        /// </summary>
        public DbSet<Tag> Tags => this.Set<Tag>();

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

            // 個別のEntityにOnModelCreatingがある場合、実行する
            var param = new object[] { modelBuilder };
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var method = entityType.ClrType.GetMethod("OnModelCreating", BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                {
                    method.Invoke(null, param);
                }
            }
        }

        /// <summary>
        /// 変更されているエンティティの登録日時/更新日時を更新する。
        /// </summary>
        private void TouchChangedEntities()
        {
            var entities = this.ChangeTracker.Entries()
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