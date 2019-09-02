// ================================================================================================
// <summary>
//      ユーザーエンティティクラスソース</summary>
//
// <copyright file="User.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Entities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// ユーザーエンティティクラス。
    /// </summary>
    public class User : IdentityUser<int>, IHasTimestamp
    {
        #region プロパティ

        /// <summary>
        /// 最終ログイン日時。
        /// </summary>
        public DateTimeOffset? LastLogin { get; set; }

        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// ユーザーのブログ。
        /// </summary>
        public ICollection<Blog> Blogs { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// モデル構築時に呼ばれる処理。
        /// </summary>
        /// <param name="modelBuilder">モデルビルダー。</param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 親クラスの列がMySQLのutf8mb4の文字数制限でエラーになるので、定義を上書き
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email).HasMaxLength(191);
                entity.Property(u => u.NormalizedEmail).HasMaxLength(191);
                entity.Property(u => u.NormalizedUserName).HasMaxLength(191);
                entity.Property(u => u.UserName).HasMaxLength(191);
            });

            // インデックスを設定
            modelBuilder.Entity<User>()
                .HasIndex(u => u.LastLogin);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.CreatedAt);
        }

        #endregion
    }
}
