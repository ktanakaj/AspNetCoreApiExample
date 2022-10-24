// ================================================================================================
// <summary>
//      アプリ独自の設定を入れたWebアプリケーションファクトリクラスソース</summary>
//
// <copyright file="CustomWebApplicationFactory.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Honememo.AspNetCoreApiExample.Dto;
using Honememo.AspNetCoreApiExample.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Honememo.AspNetCoreApiExample.Tests
{
    /// <summary>
    /// アプリ独自の設定を入れたWebアプリケーションファクトリクラス。
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        #region メンバー変数

        /// <summary>
        /// モックDBを扱うインスタンス。
        /// </summary>
        private readonly MockDb mockDb = new MockDb();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// Webアプリケーションファクトリを生成する。
        /// </summary>
        public CustomWebApplicationFactory()
        {
            // テスト用の環境変数を登録
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        }

        #endregion

        #region テスト補助用メソッド

        // ※ 以下DBコンテキストを取るメソッドが毎回newしているのは、
        //    DI管理下のものを取ると未確定のデータなども取れてしまうため。

        /// <summary>
        /// アプリDBのDBコンテキストを生成する。
        /// </summary>
        /// <returns>DBコンテキスト。</returns>
        public AppDbContext CreateDbContext()
        {
            return this.mockDb.CreateAppDbContext();
        }

        /// <summary>
        /// ユーザー認証済のHTTPクライアントを生成する。
        /// </summary>
        /// <param name="name">認証するユーザーの名前。未指定時はid=100のユーザー。</param>
        /// <param name="password">認証するユーザーのパスワード。〃。</param>
        /// <returns>HTTPクライアントと認証したユーザーのID。</returns>
        public (HttpClient Client, int Id) CreateAuthedClient(string name = "Taro", string password = "PASSWORD")
        {
            // 指定された条件でログインして、そのセッションを持つHTTPクライアントを返す
            var client = this.CreateClient();
            var body = new LoginDto() { UserName = name, Password = password };
            var response = client.PostAsJsonAsync("/api/users/login", body).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(responseString);
            }

            var node = JsonNode.Parse(responseString);
            return (client, (int)(node?["id"] ?? throw new NotImplementedException("id is not found")));
        }

        /// <summary>
        /// 新規ユーザーのHTTPクライアントを生成する。
        /// </summary>
        /// <param name="name">新規ユーザーの名前。未指定時はランダム生成。</param>
        /// <param name="password">新規ユーザーのパスワード。</param>
        /// <returns>HTTPクライアントと新規ユーザーのID。</returns>
        public (HttpClient Client, int Id) CreateNewUserClient(string name = "", string password = "TEST_PASSWORD")
        {
            // 名前が被るとエラーになるので、未指定時は動的に設定
            if (string.IsNullOrEmpty(name))
            {
                name = "NEW_USER_" + Guid.NewGuid();
            }

            // 指定された条件でユーザーを作成して、そのセッションを持つHTTPクライアントを返す
            var client = this.CreateClient();
            var body = new UserNewDto() { UserName = name, Password = password };
            var response = client.PostAsJsonAsync("/api/users", body).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(responseString);
            }

            var node = JsonNode.Parse(responseString);
            return (client, (int)(node?["id"] ?? throw new NotImplementedException("id is not found")));
        }

        #endregion

        #region アプリ設定メソッド

        /// <summary>
        /// Webホストの設定を行う。
        /// </summary>
        /// <param name="builder">Webホストのビルダー。</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // DBをテスト用に設定
                this.mockDb.ConfigureTestDb(services);
            });
        }

        #endregion
    }
}
