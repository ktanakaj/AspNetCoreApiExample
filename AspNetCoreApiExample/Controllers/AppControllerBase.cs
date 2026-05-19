// ================================================================================================
// <summary>
//      アプリコントローラ基底クラスソース</summary>
//
// <copyright file="AppControllerBase.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Honememo.AspNetCoreApiExample.Controllers;

/// <summary>
/// アプリコントローラ基底クラス。
/// </summary>
public abstract class AppControllerBase : ControllerBase
{
    /// <summary>
    /// 認証中のユーザーのIDを取得する。
    /// </summary>
    protected int UserId
    {
        get
        {
            return int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Unauthorized"));
        }
    }
}
