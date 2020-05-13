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
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// ユーザーエンティティクラス。
    /// </summary>
    public class User : IHasTimestamp
    {
        #region プロパティ

        /// <summary>
        /// ユーザーID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string UserName { get; set; }

        /// <summary>
        /// パスワード。
        /// </summary>
        /// <remarks>※ サンプルなので一旦平文。本来はハッシュ化した値を格納する。</remarks>
        [MaxLength(191)]
        public string Password { get; set; }

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
            // インデックスを設定
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.LastLogin);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.CreatedAt);
        }

        #endregion
    }
}
