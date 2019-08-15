// ================================================================================================
// <summary>
//      ブログDTOクラスソース</summary>
//
// <copyright file="BlogDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using System.ComponentModel.DataAnnotations;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// ブログDTOクラス。
    /// </summary>
    /// <remarks><see cref="Blog"/>エンティティに対応するDTO。</remarks>
    public class BlogDto : BlogEditDto
    {
        /// <summary>
        /// ブログID。
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// ブログ作者のユーザーID。
        /// </summary>
        [Required]
        public int UserId { get; set; }
    }
}
