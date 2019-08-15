// ================================================================================================
// <summary>
//      ブログ記事編集のリクエストパラメータ用のDTOクラスソース</summary>
//
// <copyright file="ArticleEditDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ブログ記事編集のリクエストパラメータ用のDTOクラス。
    /// </summary>
    public class ArticleEditDto
    {
        /// <summary>
        /// ブログ記事タイトル。
        /// </summary>
        [Required]
        [MaxLength(191)]
        public string Subject { get; set; }

        /// <summary>
        /// ブログ記事本文。
        /// </summary>
        [Required]
        [MaxLength(65535)]
        public string Body { get; set; }
    }
}
