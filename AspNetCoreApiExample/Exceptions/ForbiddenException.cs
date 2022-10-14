// ================================================================================================
// <summary>
//      権限無しの例外クラスソース</summary>
//
// <copyright file="ForbiddenException.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Exceptions
{
    /// <summary>
    /// 権限無しの例外クラス。
    /// </summary>
    public class ForbiddenException : Exception
    {
        /// <summary>
        /// 渡されたエラーメッセージで権限無しの例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        public ForbiddenException(string message)
            : base(message)
        {
        }
    }
}
