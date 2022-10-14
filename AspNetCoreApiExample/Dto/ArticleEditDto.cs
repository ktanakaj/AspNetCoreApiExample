// ================================================================================================
// <summary>
//      ブログ記事編集のリクエストパラメータ用のDTOクラスソース</summary>
//
// <copyright file="ArticleEditDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ブログ記事編集のリクエストパラメータ用のDTOクラス。
    /// </summary>
    public class ArticleEditDto
    {
        /// <summary>
        /// ブログ記事タイトル。
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// ブログ記事本文。
        /// </summary>
        [Required]
        [MaxLength(65535)]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 記事に付けられたタグ。
        /// </summary>
        [Required]
        [MaxLength(10)]
        public ICollection<string> Tags { get; set; } = new string[0];
    }
}
