// ================================================================================================
// <summary>
//      アプリ独自の設定を入れたWebアプリケーションファクトリクラスソース</summary>
//
// <copyright file="CustomWebApplicationFactory.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Tests
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// アプリ独自の設定を入れたWebアプリケーションファクトリクラス。
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        #region 定数

        /// <summary>
        /// インメモリDBのルート。
        /// </summary>
        private readonly InMemoryDatabaseRoot inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        /// <summary>
        /// WebアプリのインメモリDB名。
        /// </summary>
        private readonly string appDbName = "TestAppDB";

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

        /// <summary>
        /// WebアプリのDBコンテキストを生成する。
        /// </summary>
        /// <returns>DBコンテキスト。</returns>
        public AppDbContext CreateDbContext()
        {
            return new AppDbContext(this.ApplyTestDbConfig(new DbContextOptionsBuilder<AppDbContext>(), this.appDbName).Options);
        }

        /// <summary>
        /// ユーザー認証済のHTTPクライアントを生成する。
        /// </summary>
        /// <param name="name">認証するユーザーの名前。未指定時はid=100のユーザー。</param>
        /// <param name="password">認証するユーザーのパスワード。〃。</param>
        /// <returns>HTTPクライアントと認証したユーザーのID。</returns>
        public (HttpClient, int) CreateAuthedClient(string name = "Taro", string password = "PASSWORD")
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

            using (var doc = JsonDocument.Parse(responseString))
            {
                return (client, doc.RootElement.GetProperty("id").GetInt32());
            }
        }

        /// <summary>
        /// 新規ユーザーのHTTPクライアントを生成する。
        /// </summary>
        /// <param name="name">新規ユーザーの名前。未指定時はランダム生成。</param>
        /// <param name="password">新規ユーザーのパスワード。</param>
        /// <returns>HTTPクライアントと新規ユーザーのID。</returns>
        public (HttpClient, int) CreateNewUserClient(string name = null, string password = "TEST_PASSWORD")
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

            using (var doc = JsonDocument.Parse(responseString))
            {
                return (client, doc.RootElement.GetProperty("id").GetInt32());
            }
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
                // DBをインメモリDBに置き換え
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContextPool<AppDbContext>(options => this.ApplyTestDbConfig(options, this.appDbName));

                // データベースに汎用のテストデータを登録
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                    TestData.InitializeDbForTests(db);
                }
            });
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// DBオプションビルダーにテスト用のDB設定値を適用する。
        /// </summary>
        /// <param name="builder">ビルダー。</param>
        /// <param name="dbname">DB名。</param>
        /// <returns>メソッドチェーン用のビルダー。</returns>
        private T ApplyTestDbConfig<T>(T builder, string dbname)
            where T : DbContextOptionsBuilder
        {
            builder.UseInMemoryDatabase(dbname, this.inMemoryDatabaseRoot);
            builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            return builder;
        }

        #endregion
    }
}
