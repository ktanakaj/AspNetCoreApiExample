// ================================================================================================
// <summary>
//      データ未存在の例外クラスソース</summary>
//
// <copyright file="NotFoundException.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Exceptions
{
    /// <summary>
    /// データ未存在の例外クラス。
    /// </summary>
    public class NotFoundException : Exception
    {
        #region コンストラクタ

        /// <summary>
        /// 渡されたエラーメッセージと追加情報でデータ未存在の例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="data">エラーの追加情報。</param>
        public NotFoundException(string message, System.Collections.IDictionary? data = null)
            : base(message)
        {
            this.Data = data ?? new Dictionary<string, object>();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 追加情報。
        /// </summary>
        public override System.Collections.IDictionary Data { get; }

        #endregion
    }
}
