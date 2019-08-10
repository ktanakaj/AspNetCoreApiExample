// ================================================================================================
// <summary>
//      アプリコントローラ基底クラスソース</summary>
//
// <copyright file="AppControllerBase.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;

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
                return int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
        }

        #endregion
    }
}
