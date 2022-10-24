// ================================================================================================
// <summary>
//      更新日時を持つエンティティのインタフェースソース</summary>
//
// <copyright file="IHasUpdatedAt.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Entities
{
    /// <summary>
    /// 更新日時を持つエンティティのインタフェース。
    /// </summary>
    /// <remarks>このインタフェースを実装している場合、各DBコンテキストで自動的に更新日時が更新されます。</remarks>
    public interface IHasUpdatedAt
    {
        /// <summary>
        /// 更新日時。
        /// </summary>
        DateTimeOffset UpdatedAt { get; set; }
    }
}
