// ================================================================================================
// <summary>
//      ミドルウェアを無効にする場合のターゲットソース</summary>
//
// <copyright file="DisableTarget.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Middlewares
{
    /// <summary>
    /// ミドルウェアを無効にする場合のターゲット。
    /// </summary>
    public enum DisableTarget
    {
        /// <summary>
        /// リクエスト/レスポンス双方。
        /// </summary>
        All,

        /// <summary>
        /// リクエストのみ無効。
        /// </summary>
        Request,

        /// <summary>
        /// レスポンスのみ無効。
        /// </summary>
        Response,
    }
}
