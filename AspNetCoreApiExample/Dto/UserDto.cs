// ================================================================================================
// <summary>
//      ユーザーDTOクラスソース</summary>
//
// <copyright file="UserDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;
using Honememo.AspNetCoreApiExample.Entities;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ユーザーDTOクラス。
    /// </summary>
    /// <remarks><see cref="User"/>エンティティに対応するDTO。</remarks>
    public class UserDto : UserEditDto
    {
        /// <summary>
        /// ユーザーID。
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 最終ログイン日時。
        /// </summary>
        public DateTimeOffset? LastLogin { get; set; }

        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// 更新日時。
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
