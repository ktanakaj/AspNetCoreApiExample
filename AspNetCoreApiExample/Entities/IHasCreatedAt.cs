// ================================================================================================
// <summary>
//      作成日時を持つエンティティのインタフェースソース</summary>
//
// <copyright file="IHasCreatedAt.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Entities
{
    /// <summary>
    /// 作成日時を持つエンティティのインタフェース。
    /// </summary>
    /// <remarks>このインタフェースを実装している場合、各DBコンテキストで自動的に作成日時が入ります。</remarks>
    public interface IHasCreatedAt
    {
        /// <summary>
        /// 作成日時。
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }
    }
}
