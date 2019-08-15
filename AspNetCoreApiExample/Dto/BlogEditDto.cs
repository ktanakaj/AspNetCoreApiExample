// ================================================================================================
// <summary>
//      ブログ登録/編集のリクエストパラメータ用のDTOクラスソース</summary>
//
// <copyright file="BlogEditDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ブログ登録/編集のリクエストパラメータ用のDTOクラス。
    /// </summary>
    public class BlogEditDto
    {
        /// <summary>
        /// ブログタイトル。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string Name { get; set; }
    }
}
