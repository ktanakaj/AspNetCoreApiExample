// ================================================================================================
// <summary>
//      リクエスト/レスポンスバッファリング無効化属性ソース</summary>
//
// <copyright file="DisableBufferingAttribute.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Middlewares
{
    /// <summary>
    /// リクエスト/レスポンスバッファリング無効化属性。
    /// </summary>
    /// <remarks><see cref="EnableBufferingMiddleware"/>を適用したくないAPIに付加する属性。</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class DisableBufferingAttribute : Attribute
    {
        #region メンバー変数

        /// <summary>
        /// 無効化するターゲット。
        /// </summary>
        private readonly DisableTarget target;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたターゲットへのバッファリングを無効にする。
        /// </summary>
        /// <param name="target">無効化するターゲット。未指定時は全て。</param>
        public DisableBufferingAttribute(DisableTarget target = DisableTarget.All)
        {
            this.target = target;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// リクエストのバッファリングを無効にするか？
        /// </summary>
        /// <returns>無効にする場合true。</returns>
        public bool IsRequestDisabled()
        {
            return this.target == DisableTarget.All || this.target == DisableTarget.Request;
        }

        /// <summary>
        /// レスポンスのバッファリングを無効にするか？
        /// </summary>
        /// <returns>無効にする場合true。</returns>
        public bool IsResponseDisabled()
        {
            return this.target == DisableTarget.All || this.target == DisableTarget.Response;
        }

        #endregion
    }
}
