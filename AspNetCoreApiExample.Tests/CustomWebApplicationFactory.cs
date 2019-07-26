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
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// アプリ独自の設定を入れたWebアプリケーションファクトリクラス。
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        #region 静的変数

        /// <summary>
        /// インメモリDBのルート。
        /// </summary>
        public static readonly InMemoryDatabaseRoot InMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        #endregion

        #region メンバー変数

        /// <summary>
        /// WebアプリのインメモリDB名。
        /// </summary>
        private readonly string appDbName = "TestBlogDB";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// Webアプリケーションファクトリを生成する。
        /// </summary>
        public CustomWebApplicationFactory()
        {
            // 現状の造りだと、Factoryが作られるたび(?)に
            // テストデータの登録処理も動き、ID重複などが起こるため、
            // DB名にGUIDを付けて処理ごとに一意にする。
            // （一度しか作らないようにする手もあるが、そうするとパラレルで動かせなくなるので）
            this.appDbName += Guid.NewGuid();

            // テスト用の環境変数を登録
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            }
        }

        #endregion

        #region テスト補助用メソッド

        /// <summary>
        /// WebアプリのDBコンテキストを生成する。
        /// </summary>
        /// <returns>DBコンテキスト。</returns>
        public AppDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase(this.appDbName, InMemoryDatabaseRoot);
            builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            return new AppDbContext(builder.Options);
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
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(this.appDbName, InMemoryDatabaseRoot);
                    options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });

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
    }
}
