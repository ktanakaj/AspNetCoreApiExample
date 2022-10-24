// ================================================================================================
// <summary>
//      IDを持つエンティティのインタフェースソース</summary>
//
// <copyright file="IHasId.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Entities
{
    /// <summary>
    /// IDを持つエンティティのインタフェース。
    /// </summary>
    /// <typeparam name="T">IDの型。</typeparam>
    /// <remarks>共通処理作成用。共通処理を使用しない場合、インタフェースを使わなくても問題無いです。</remarks>
    public interface IHasId<T>
        where T : notnull
    {
        /// <summary>
        /// エンティティを一意に特定するID。
        /// </summary>
        T Id { get; }
    }
}
