// ================================================================================================
// <summary>
//      ログインのリクエストパラメータDTOクラスソース</summary>
//
// <copyright file="LoginDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ログインのリクエストパラメータDTOクラス。
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// ユーザー名。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string UserName { get; set; }

        /// <summary>
        /// パスワード。
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
