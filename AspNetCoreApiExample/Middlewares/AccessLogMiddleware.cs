// ================================================================================================
// <summary>
//      アクセスログミドルウェアクラスソース</summary>
//
// <copyright file="AccessLogMiddleware.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Middlewares
{
    using System;
    using System.IO;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// アクセスログミドルウェアクラス。
    /// </summary>
    /// <remarks><see cref="EnableBufferingMiddleware"/>が先に適用されている必要有。</remarks>
    public class AccessLogMiddleware
    {
        #region メンバー変数

        /// <summary>
        /// 次の処理のデリゲート。
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 環境情報。
        /// </summary>
        private readonly IHostEnvironment env;

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<AccessLogMiddleware> logger;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ミドルウェアを生成する。
        /// </summary>
        /// <param name="next">次の処理のデリゲート。</param>
        /// <param name="env">環境情報。</param>
        /// <param name="logger">ロガー。</param>
        public AccessLogMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<AccessLogMiddleware> logger)
        {
            this.next = next;
            this.env = env;
            this.logger = logger;
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
            var starttime = DateTimeOffset.Now;
            try
            {
                await this.next(context);
            }
            finally
            {
                await this.Log(context, starttime);
            }
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// ログを出力する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <param name="starttime">処理開始時間。</param>
        /// <returns>処理状態。</returns>
        private async Task Log(HttpContext context, DateTimeOffset starttime)
        {
            // ※ EnableBufferingMiddlewareでリクエストボディは再読み込み可、
            //    かつレスポンスボディはMemoryStreamとなっている前提。
            try
            {
                var req = context.Request;
                var res = context.Response;
                res.Body.Position = 0;
                string resBody = await new StreamReader(res.Body).ReadToEndAsync();

                // ※ 以下、CombinedLogに+αのデバッグ情報を載せて出力。
                //    適当にデバッグ用途メインに決めたものなので、
                //    KPIなどでアクセスログが欲しい場合は、丸々置き換える。

                // 基本のアクセスログ
                // TODO: プロトコル（HTTP/1.1みたいな奴）を取る方法が不明なためスキーマで代用している
                string log = context.Connection.RemoteIpAddress?.ToString() + " - - \"" + req.Method
                    + " " + req.GetDisplayUrl() + " " + req.Scheme.ToUpper() + "\" " + res.StatusCode
                    + " " + resBody.Length + " \""
                    + (req.Headers.ContainsKey("Referer") ? req.Headers["Referer"].ToString() : string.Empty) + "\" \""
                    + (req.Headers.ContainsKey("User-Agent") ? req.Headers["User-Agent"].ToString() : string.Empty) + "\"";

                // 処理時間
                if (starttime != null)
                {
                    log += " " + (DateTimeOffset.Now.ToUnixTimeMilliseconds() - starttime.ToUnixTimeMilliseconds()) + "ms";
                }

                // ユーザーID
                if (context.User.Identity.IsAuthenticated)
                {
                    log += " id=" + context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }

                // リクエストボディ/レスポンスボディ
                if (this.env.IsDevelopment())
                {
                    req.Body.Position = 0;
                    string reqBody = await new StreamReader(req.Body).ReadToEndAsync();
                    if (req.ContentType != null && req.ContentType.Contains("json"))
                    {
                        reqBody = this.HidePasswordLog(reqBody);
                    }

                    if (res.ContentType != null && res.ContentType.Contains("json"))
                    {
                        resBody = this.HidePasswordLog(resBody);
                    }

                    log += " req=" + reqBody + " res=" + resBody;
                }

                // ステータスコードによってログレベルを切り替え
                int status = res.StatusCode;
                if (status >= 500)
                {
                    this.logger.LogError(log);
                }
                else if (status >= 400)
                {
                    this.logger.LogWarning(log);
                }
                else
                {
                    this.logger.LogInformation(log);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(0, e, "Access log failed");
            }
        }

        /// <summary>
        /// JSONログ上のパスワードを隠す。
        /// </summary>
        /// <param name="log">JSONログ文字列。</param>
        /// <returns>パスワードを置換したログ文字列。</returns>
        private string HidePasswordLog(string log)
        {
            // TODO: パスワードに"が含まれていると中途半端になるかも
            if (log == null)
            {
                return log;
            }

            return Regex.Replace(log, "(\"(password|currentPassword|newPassword)\"\\s*:\\s*)\".*?\"", "$1\"****\"");
        }

        #endregion
    }
}
