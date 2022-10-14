// ================================================================================================
// <summary>
//      ブログエンティティクラスソース</summary>
//
// <copyright file="Blog.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Entities
{
    /// <summary>
    /// ブログエンティティクラス。
    /// </summary>
    [Index(nameof(Name))]
    [Index(nameof(CreatedAt))]
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
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

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
        public User User { get; set; } = null!;

        /// <summary>
        /// ブログの記事。
        /// </summary>
        public ICollection<Article> Articles { get; set; } = null!;

        #endregion
    }
}
