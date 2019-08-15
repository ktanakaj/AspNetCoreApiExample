﻿// ================================================================================================
// <summary>
//      ブログ記事DTOクラスソース</summary>
//
// <copyright file="ArticleDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// ブログ記事DTOクラス。
    /// </summary>
    /// <remarks><see cref="Article"/>エンティティに対応するDTO。</remarks>
    public class ArticleDto : ArticleNewDto
    {
        /// <summary>
        /// ブログ記事ID。
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
}
