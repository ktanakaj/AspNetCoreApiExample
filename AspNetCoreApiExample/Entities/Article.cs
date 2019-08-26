// ================================================================================================
// <summary>
//      ブログ記事エンティティクラスソース</summary>
//
// <copyright file="Article.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ブログ記事エンティティクラス。
    /// </summary>
    public class Article : IHasTimestamp
    {
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
        [MaxLength(191)]
        public string Subject { get; set; }

        /// <summary>
        /// ブログ記事本文。
        /// </summary>
        [Required]
        [MaxLength(65535)]
        public string Body { get; set; }

        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        ///// <summary>
        ///// ブログ記事タグ。
        ///// </summary>
        // public string[] Tags { get; set; }

        /// <summary>
        /// ブログ。
        /// </summary>
        public Blog Blog { get; set; }
    }
}
