// ================================================================================================
// <summary>
//      アプリコントローラ基底クラスソース</summary>
//
// <copyright file="AppControllerBase.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Security.Claims;
using Honememo.AspNetCoreApiExample.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Honememo.AspNetCoreApiExample.Controllers
{
    /// <summary>
    /// アプリコントローラ基底クラス。
    /// </summary>
    public abstract class AppControllerBase : ControllerBase
    {
        #region プロパティ

        /// <summary>
        /// 認証中のユーザーのIDを取得する。
        /// </summary>
        protected int UserId
        {
            get
            {
                return int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new AppException("Unauthorized", "UNAUTHORIZED"));
            }
        }

        #endregion
    }
}
