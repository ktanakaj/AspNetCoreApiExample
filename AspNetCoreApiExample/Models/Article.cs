// ================================================================================================
// <summary>
//      ブログ記事モデルクラスソース</summary>
//
// <copyright file="Article.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ブログ記事モデルクラス。
    /// </summary>
    public class Article
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
        public string Subject { get; set; }

        /// <summary>
        /// ブログ記事本文。
        /// </summary>
        [Required]
        public string Body { get; set; }

        ///// <summary>
        ///// ブログ記事タグ。
        ///// </summary>
        // public string[] Tags { get; set; }
    }
}
