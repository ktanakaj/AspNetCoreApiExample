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
        public string Name { get; set; }

        /// <summary>
        /// パスワード。
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
