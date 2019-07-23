// ================================================================================================
// <summary>
//      エラー処理ミドルウェアクラスソース</summary>
//
// <copyright file="ErrorHandlingMiddleware.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Middlewares
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Honememo.AspNetCoreApiExample.Exceptions;

    /// <summary>
    /// エラー処理ミドルウェアクラス。
    /// </summary>
    public class ErrorHandlingMiddleware
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
        public ErrorHandlingMiddleware(RequestDelegate next)
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
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                await this.OnException(context, ex);
            }
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 例外を処理する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <param name="exception">発生した例外。</param>
        /// <returns>処理状態。</returns>
        private Task OnException(HttpContext context, Exception exception)
        {
            // デフォルトは500エラー、例外の種類に応じたステータスコードとエラー情報を返す
            var status = HttpStatusCode.InternalServerError;
            var err = new ErrorObject();

            // TODO: メッセージは本番環境ではそのまま返さないようにする
            err.Message = exception.Message;
            err.Data = exception.Data;
            if (exception is AppException appEx)
            {
                err.Code = appEx.Code;
            }

            // TODO: HTTPステータスコードは、ちゃんとやるならエラーコードマスタとか定義してそこから取る。
            //       マスタ定義するなら、通常例外の業務例外への変換とかもやる。
            if (exception is BadRequestException)
            {
                status = HttpStatusCode.BadRequest;
            }
            else if (exception is NotFoundException)
            {
                status = HttpStatusCode.NotFound;
            }

            var result = JsonConvert.SerializeObject(
                new { error = err },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// エラーオブジェクト。
        /// </summary>
        public class ErrorObject
        {
            /// <summary>
            /// エラーコード。
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// エラーメッセージ。
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 追加情報。
            /// </summary>
            public object Data { get; set; }
        }

        #endregion
    }
}
