// ================================================================================================
// <summary>
//      アクセスログミドルウェアクラスソース</summary>
//
// <copyright file="AccessLogMiddleware.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;

namespace Honememo.AspNetCoreApiExample.Middlewares
{
    /// <summary>
    /// アクセスログミドルウェアクラス。
    /// </summary>
    /// <remarks>
    /// リクエストボディなども含めたアプリ独自のテキスト/JSON形式のアクセスログを出力する。
    /// （<a href="https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0">標準のHTTPログミドルウェア</a>
    /// はリクエストとレスポンスで二行だったり使い勝手が悪いので。）
    /// ボディの出力は先に<see cref="EnableBufferingMiddleware"/>が適用されている必要有。
    /// </remarks>
    public class AccessLogMiddleware
    {
        #region 定数

        /// <summary>
        /// ログに出力するコンテンツ種別（文字列完全一致 or 正規表現）。
        /// </summary>
        private static readonly object[] ContentTypes =
            {
                new Regex("^application/json"),
                new Regex("^application/problem"),
                new Regex("^text/"),
                new Regex("^multipart/form-data"),
                "application/xml",
                "application/javascript",
                "application/x-javascript",
                "application/x-www-form-urlencoded",
            };

        #endregion

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
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        /// HTTPのアクセスログを出力する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <param name="starttime">処理開始時間。</param>
        /// <returns>処理状態。</returns>
        private async Task Log(HttpContext context, DateTimeOffset starttime)
        {
            try
            {
                // ステータスコードによってログレベルを切り替え
                var status = context.Response.StatusCode;
                var lv = LogLevel.Information;
                if (status >= 500)
                {
                    lv = LogLevel.Error;
                }
                else if (status >= 400)
                {
                    lv = LogLevel.Warning;
                }

                // CombinedLogに+αのデバッグ情報を載せたログを生成
                // TODO: プロトコル（HTTP/1.1みたいな奴）を取る方法が不明なためスキーマで代用している。
                // TODO: ユーザーIDやレスポンス長がnullの場合に '-' とかにしたいが未解決。
                //      （値の方を変えると、JSONとかにした際にそっちも変わってしまうのでテンプレート側で解決したい。）
                var req = context.Request;
                var res = context.Response;
                var template = "{IpAddress:l} - {UserId} \"{RequestMethod:l} {RequestUrl:l} {Scheme}\" {StatusCode} {ResponseLength} \"{Referrer:l}\" \"{UserAgent:l}\" {Elapsed}ms";
                var param = new List<object?>
                {
                    context.Connection.RemoteIpAddress?.ToString(),
                    this.GetUserId(context),
                    req.Method,
                    req.GetDisplayUrl(),
                    req.Scheme.ToUpper(),
                    res.StatusCode,
                    res.ContentLength,
                    this.GetReferer(req.Headers),
                    this.GetUserAgent(req.Headers),
                    (DateTimeOffset.UtcNow - starttime).TotalMilliseconds,
                };

                // 開発環境ではリクエストボディ/レスポンスボディも出力
                if (this.env.IsDevelopment())
                {
                    template += " req={RequestBody} res={ResponseBody}";
                    param.Add(await this.GetRequestBody(context));
                    param.Add(await this.GetResponseBody(context));
                }

                // ログ出力
                this.logger.Log(lv, template, param.ToArray());
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Access log failed");
            }
        }

        /// <summary>
        /// リクエストボディを取得する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>リクエストボディの文字列。</returns>
        private async Task<string> GetRequestBody(HttpContext context)
        {
            var body = string.Empty;
            var req = context.Request;
            if (req.ContentLength > 0)
            {
                if (!EnableBufferingMiddleware.IsRequestBufferingEnabled(context))
                {
                    body = $"(not loggable)";
                }
                else if (!this.IsLoggableBody(req.ContentType))
                {
                    body = $"({req.ContentType})";
                }
                else
                {
                    body = await this.ReadStream(req.Body);
                    if (this.IsJsonBody(req.ContentType))
                    {
                        body = this.FormatJsonBody(body);
                    }
                }
            }

            return body;
        }

        /// <summary>
        /// レスポンスボディを取得する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>レスポンスボディの文字列。</returns>
        private async Task<string> GetResponseBody(HttpContext context)
        {
            // ※ ContentLength はデータがあるのに入っていないこともあるので、とりあえず処理してみる
            var body = string.Empty;
            var res = context.Response;
            if (res.ContentLength == null || res.ContentLength > 0)
            {
                if (!EnableBufferingMiddleware.IsResponseBufferingEnabled(context))
                {
                    body = $"(not loggable)";
                }
                else if (string.IsNullOrEmpty(res.ContentType))
                {
                    // ※ ContentLength=null で本当に空の場合にここに来る
                }
                else if (!this.IsLoggableBody(res.ContentType))
                {
                    body = $"({res.ContentType})";
                }
                else
                {
                    body = await this.ReadStream(res.Body);
                    if (this.IsJsonBody(res.ContentType))
                    {
                        body = this.FormatJsonBody(body);
                    }
                }
            }

            return body;
        }

        /// <summary>
        /// ストリームを先頭に移動して読み込む。
        /// </summary>
        /// <param name="stream">読み込むストリーム。</param>
        /// <returns>ストリームの中身の文字列。</returns>
        /// <remarks>
        /// <see cref="EnableBufferingMiddleware"/>でリクエストボディは再読み込み可、
        /// かつレスポンスボディはMemoryStreamとなっている前提。
        /// </remarks>
        private async Task<string> ReadStream(Stream stream)
        {
            stream.Position = 0;
            var text = await new StreamReader(stream).ReadToEndAsync();
            stream.Position = 0;
            return text;
        }

        /// <summary>
        /// ログ出力可能なコンテンツ種別か？
        /// </summary>
        /// <param name="type">コンテンツ種別。</param>
        /// <returns>可能な場合true。</returns>
        private bool IsLoggableBody(string? type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return false;
            }

            foreach (var t in ContentTypes)
            {
                if (t is Regex regex)
                {
                    if (regex.IsMatch(type))
                    {
                        return true;
                    }
                }
                else
                {
                    if (type.Equals(t))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// JSON系のコンテンツ種別か？
        /// </summary>
        /// <param name="type">コンテンツ種別。</param>
        /// <returns>JSON系の場合true。</returns>
        private bool IsJsonBody(string? type)
        {
            return !string.IsNullOrEmpty(type) && (type.Contains("/json") || type.Contains("+json"));
        }

        /// <summary>
        /// JSONパラメータをログ出力用に成形する。
        /// </summary>
        /// <param name="log">加工するJSON文字列。</param>
        /// <returns>加工したJSONオブジェクト。</returns>
        private string FormatJsonBody(string log)
        {
            // JSONログ上のパスワードを隠す
            // TODO: 手抜き実装。パスワードに"が含まれていると中途半端になるかも。
            //       運用で使う場合はJsonNode.Parse()して再帰的にキー名をチェックする。
            if (log == null)
            {
                return string.Empty;
            }

            return Regex.Replace(log, "(\"(password|currentPassword|newPassword)\"\\s*:\\s*)\".*?\"", "$1\"****\"");
        }

        /// <summary>
        /// リクエストヘッダーからユーザーエージェントを取得する。
        /// </summary>
        /// <param name="headers">ヘッダー。</param>
        /// <returns>ユーザーエージェント。取得できない場合は空文字列。</returns>
        private string GetUserAgent(IHeaderDictionary headers)
        {
            return headers.ContainsKey("User-Agent") ? headers["User-Agent"].First() : string.Empty;
        }

        /// <summary>
        /// リクエストヘッダーからリファラーを取得する。
        /// </summary>
        /// <param name="headers">ヘッダー。</param>
        /// <returns>リファラー。取得できない場合は空文字列。</returns>
        private string GetReferer(IHeaderDictionary headers)
        {
            return headers.ContainsKey("Referer") ? headers["Referer"].First() : string.Empty;
        }

        /// <summary>
        /// ユーザーIDを取得する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>ユーザーID。取得できない場合null。</returns>
        private int? GetUserId(HttpContext context)
        {
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        #endregion
    }
}
