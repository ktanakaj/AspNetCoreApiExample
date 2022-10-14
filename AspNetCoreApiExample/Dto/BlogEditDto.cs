// ================================================================================================
// <summary>
//      ブログ登録/編集のリクエストパラメータ用のDTOクラスソース</summary>
//
// <copyright file="BlogEditDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ブログ登録/編集のリクエストパラメータ用のDTOクラス。
    /// </summary>
    public class BlogEditDto
    {
        /// <summary>
        /// ブログタイトル。
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}
