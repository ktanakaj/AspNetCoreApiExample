// ================================================================================================
// <summary>
//      パスワード変更のリクエストパラメータDTOクラスソース</summary>
//
// <copyright file="ChangePasswordDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// パスワード変更のリクエストパラメータDTOクラス。
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 現在のパスワード。
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// 新しいパスワード。
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }
}
