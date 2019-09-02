// ================================================================================================
// <summary>
//      ブログエンティティクラスソース</summary>
//
// <copyright file="Blog.cs">
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
    /// ブログエンティティクラス。
    /// </summary>
    public class Blog : IHasTimestamp
    {
        #region プロパティ

        /// <summary>
        /// ブログID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ブログタイトル。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string Name { get; set; }

        /// <summary>
        /// ブログ作者のユーザーID。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// ブログ作者のユーザー。
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// ブログの記事。
        /// </summary>
        public ICollection<Article> Articles { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// モデル構築時に呼ばれる処理。
        /// </summary>
        /// <param name="modelBuilder">モデルビルダー。</param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            // インデックスを設定
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.Name);
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.CreatedAt);
        }

        #endregion
    }
}
