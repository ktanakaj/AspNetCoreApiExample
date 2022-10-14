// ================================================================================================
// <summary>
//      リクエスト/レスポンスバッファリングミドルウェアクラスソース</summary>
//
// <copyright file="EnableBufferingMiddleware.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Middlewares
{
    /// <summary>
    /// リクエスト/レスポンスバッファリングミドルウェアクラス。
    /// </summary>
    /// <remarks>
    /// リクエストのEnableBuffering有効化と、レスポンスボディの<see cref="MemoryStream"/>への差し替えを行う。
    /// </remarks>
    public class EnableBufferingMiddleware
    {
        #region メンバー変数

        /// <summary>
        /// 次の処理のデリゲート。
        /// </summary>
        private readonly RequestDelegate next;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ミドルウェアを生成する。
        /// </summary>
        /// <param name="next">次の処理のデリゲート。</param>
        public EnableBufferingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        #endregion

        #region ミドルウェアメソッド

        /// <summary>
        /// ミドルウェアを実行する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>処理状態。</returns>
        public async Task Invoke(HttpContext context)
        {
            // リクエストのバッファリングを有効化。
            context.Request.EnableBuffering();

            // レスポンスは設定だけではできないので、本来のレスポンスのストリームを
            // 一時的にメモリストリームに差し替えることでバッファリングする。
            var responseStream = context.Response.Body;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // メモリストリームで処理を実行
                    context.Response.Body = memoryStream;
                    await this.next(context);

                    // メモリ上のレスポンスボディを本来のレスポンスに出力
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(responseStream);
                }
            }
            finally
            {
                // レスポンスを本物に戻す
                context.Response.Body = responseStream;
            }
        }

        #endregion
    }
}
