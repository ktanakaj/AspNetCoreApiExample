// ================================================================================================
// <summary>
//      ユーザーエンティティクラスソース</summary>
//
// <copyright file="User.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Entities
{
    /// <summary>
    /// ユーザーエンティティクラス。
    /// </summary>
    [Index(nameof(LastLogin))]
    [Index(nameof(CreatedAt))]
    public class User : IdentityUser<int>, IHasId<int>, IHasCreatedAt, IHasUpdatedAt
    {
        #region DB列のプロパティ

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

        #endregion

        #region リレーションプロパティ

        /// <summary>
        /// ユーザーのブログ。
        /// </summary>
        public ICollection<Blog> Blogs { get; set; } = null!;

        #endregion
    }
}
