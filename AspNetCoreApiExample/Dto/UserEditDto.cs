// ================================================================================================
// <summary>
//      ユーザー更新のリクエストパラメータDTOクラスソース</summary>
//
// <copyright file="UserEditDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ユーザー更新のリクエストパラメータDTOクラス。
    /// </summary>
    public class UserEditDto
    {
        /// <summary>
        /// ユーザー名。
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; } = string.Empty;
    }
}
