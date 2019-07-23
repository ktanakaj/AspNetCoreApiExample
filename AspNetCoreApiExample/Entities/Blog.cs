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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ブログエンティティクラス。
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// ブログID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ブログタイトル。
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// ブログ作者のユーザーID。
        /// </summary>
        public int UserId { get; set; }
    }
}
