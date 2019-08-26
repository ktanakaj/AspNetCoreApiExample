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

    /// <summary>
    /// ブログエンティティクラス。
    /// </summary>
    public class Blog : IHasTimestamp
    {
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
    }
}
