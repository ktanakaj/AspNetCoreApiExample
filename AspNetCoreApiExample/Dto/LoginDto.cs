// ================================================================================================
// <summary>
//      ログインのリクエストパラメータDTOクラスソース</summary>
//
// <copyright file="LoginDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ログインのリクエストパラメータDTOクラス。
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// ユーザー名。
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// パスワード。
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
