// ================================================================================================
// <summary>
//      ブログ記事登録のリクエストパラメータ用のDTOクラスソース</summary>
//
// <copyright file="ArticleNewDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ブログ記事登録のリクエストパラメータ用のDTOクラス。
    /// </summary>
    public class ArticleNewDto : ArticleEditDto
    {
        /// <summary>
        /// ブログID。
        /// </summary>
        [Required]
        public int BlogId { get; set; }
    }
}
