// ================================================================================================
// <summary>
//      アプリサーバーのコントローラのテスト用の基底クラスソース</summary>
//
// <copyright file="ControllerTestBase.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Honememo.AspNetCoreApiExample.Tests.Controllers
{
    /// <summary>
    /// アプリサーバーのコントローラのテスト用の基底クラス。
    /// </summary>
    /// <remarks>テストの共通処理や便利メソッドを実装。必要に応じて使ってください。</remarks>
    public abstract class ControllerTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        #region コンストラクタ

        /// <summary>
        /// Webアプリのファクトリクラスを使用するテストインスタンスを生成する。
        /// </summary>
        /// <param name="factory">Webアプリファクトリクラス。</param>
        public ControllerTestBase(CustomWebApplicationFactory factory)
        {
            this.Factory = factory;
            this.Client = factory.CreateClient();
            (this.AuthedClient, this.UserId) = factory.CreateAuthedClient();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// Webアプリのファクトリー。
        /// </summary>
        protected CustomWebApplicationFactory Factory { get; }

        /// <summary>
        /// Webアプリテスト用のHTTPクライアント。
        /// </summary>
        protected HttpClient Client { get; set; }

        /// <summary>
        /// Webアプリテスト用の認証済HTTPクライアント。
        /// </summary>
        protected HttpClient AuthedClient { get; set; }

        /// <summary>
        /// 認証済HTTPクライアントのユーザーID。
        /// </summary>
        protected int UserId { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// レスポンスが成功しているかを検証する。
        /// </summary>
        /// <param name="response">検証するレスポンス。</param>
        /// <returns>処理状態。</returns>
        internal static async Task AssertResponse(HttpResponseMessage response)
        {
            // ただ検証するだけだとエラーが分かり難いので、メッセージを付加する
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"Response is not success (status={response.StatusCode}, body={body})");
        }

        /// <summary>
        /// レスポンスが失敗しているかを検証する。
        /// </summary>
        /// <param name="response">検証するレスポンス。</param>
        /// <param name="statusCode">期待するステータスコード。</param>
        /// <returns>処理状態。</returns>
        internal static async Task AssertErrorResponse(HttpResponseMessage response, HttpStatusCode statusCode)
        {
            // ただ検証するだけだとエラーが分かり難いので、メッセージを付加する
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == statusCode, $"Response is not {statusCode} (status={response.StatusCode}, body={body})");
        }

        /// <summary>
        /// レスポンスボディをオブジェクトにデシリアライズして取得する。
        /// </summary>
        /// <typeparam name="T">レスポンスボディの型。</typeparam>
        /// <param name="response">取得するレスポンス。</param>
        /// <returns>取得したレスポンスボディ。</returns>
        internal static async Task<T?> GetResponseBody<T>(HttpResponseMessage response)
        {
            return JsonSerializer.Deserialize<T>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
        }

        #endregion
    }
}
