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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ユーザーエンティティクラス。
    /// </summary>
    public class User
    {
        /// <summary>
        /// ユーザーID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string Name { get; set; }

        /// <summary>
        /// パスワード。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string Password { get; set; }

        /// <summary>
        /// ユーザーのブログ。
        /// </summary>
        public ICollection<Blog> Blogs { get; set; }
    }
}
