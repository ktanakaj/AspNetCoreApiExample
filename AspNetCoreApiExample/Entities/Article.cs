// ================================================================================================
// <summary>
//      ブログ記事エンティティクラスソース</summary>
//
// <copyright file="Article.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Entities
{
    /// <summary>
    /// ブログ記事エンティティクラス。
    /// </summary>
    [Index(nameof(Subject))]
    [Index(nameof(CreatedAt))]
    public class Article : IHasTimestamp
    {
        #region プロパティ

        /// <summary>
        /// ブログ記事ID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ブログID。
        /// </summary>
        [Required]
        public int BlogId { get; set; }

        /// <summary>
        /// ブログ記事タイトル。
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// ブログ記事本文。
        /// </summary>
        [Required]
        [MaxLength(65535)]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// ブログ。
        /// </summary>
        public Blog Blog { get; set; } = null!;

        /// <summary>
        /// ブログ記事タグ。
        /// </summary>
        public ICollection<Tag> Tags { get; set; } = null!;

        #endregion
    }
}
