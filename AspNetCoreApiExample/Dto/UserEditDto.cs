// ================================================================================================
// <summary>
//      ユーザー更新のリクエストパラメータDTOクラスソース</summary>
//
// <copyright file="UserEditDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ユーザー更新のリクエストパラメータDTOクラス。
    /// </summary>
    public class UserEditDto
    {
        /// <summary>
        /// ユーザー名。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string UserName { get; set; }
    }
}
