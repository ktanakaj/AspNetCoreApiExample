// ================================================================================================
// <summary>
//      DB処理単位集約用インタフェースソース</summary>
//
// <copyright file="IUnitOfWork.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Microsoft.EntityFrameworkCore.Storage;

namespace Honememo.AspNetCoreApiExample.Repositories
{
    /// <summary>
    /// DB処理単位集約用インタフェース。
    /// </summary>
    /// <remarks>
    /// 現状いまいち使いどころが分からないので、単にController/Service層でトランザクション時に、
    /// 生のDbContextを見なくて済むためのinterfaceとして定義。
    /// </remarks>
    public interface IUnitOfWork
    {
        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        /// <returns>トランザクション。</returns>
        IDbContextTransaction BeginTransaction();
    }
}