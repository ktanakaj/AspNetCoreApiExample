// ================================================================================================
// <summary>
//      パスワード変更のリクエストパラメータDTOクラスソース</summary>
//
// <copyright file="ChangePasswordDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// パスワード変更のリクエストパラメータDTOクラス。
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 現在のパスワード。
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新しいパスワード。
        /// </summary>
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
