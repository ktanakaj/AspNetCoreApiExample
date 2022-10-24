// ================================================================================================
// <summary>
//      リクエスト/レスポンスバッファリングミドルウェアクラスソース</summary>
//
// <copyright file="EnableBufferingMiddleware.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
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
    /// 特定のAPIだけバッファリングを行いたくない場合は、<see cref="DisableBufferingAttribute"/>で除外もできます。
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
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        #endregion

        #region 実装支援用メソッド

        /// <summary>
        /// このHTTPコンテキストでリクエストのバッファリングが有効か？
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>有効な場合true。</returns>
        public static bool IsRequestBufferingEnabled(HttpContext context)
        {
            var disablingAttr = context.GetEndpoint()?.Metadata.GetMetadata<DisableBufferingAttribute>();
            return disablingAttr == null || !disablingAttr.IsRequestDisabled();
        }

        /// <summary>
        /// このHTTPコンテキストでレスポンスのバッファリングが有効か？
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>有効な場合true。</returns>
        public static bool IsResponseBufferingEnabled(HttpContext context)
        {
            var disablingAttr = context.GetEndpoint()?.Metadata.GetMetadata<DisableBufferingAttribute>();
            return disablingAttr == null || !disablingAttr.IsResponseDisabled();
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
            // ※ APIにバッファリング無効が指定されている場合、随時除外する

            // リクエストのバッファリング
            if (IsRequestBufferingEnabled(context))
            {
                context.Request.EnableBuffering();
            }

            // レスポンスのバッファリング
            if (IsResponseBufferingEnabled(context))
            {
                // レスポンスは設定だけではできないので、本来のレスポンスのストリームを
                // 一時的にメモリストリームに差し替えることでバッファリングする。
                var responseStream = context.Response.Body;
                try
                {
                    using var memoryStream = new MemoryStream();
                    context.Response.Body = memoryStream;
                    await this.next(context);

                    // メモリ上のレスポンスボディを本来のレスポンスに出力
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(responseStream);
                }
                finally
                {
                    // レスポンスを本物に戻す
                    context.Response.Body = responseStream;
                }
            }
            else
            {
                await this.next(context);
            }
        }

        #endregion
    }
}
