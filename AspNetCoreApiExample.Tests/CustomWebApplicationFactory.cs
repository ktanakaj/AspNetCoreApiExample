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
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// アプリ独自の設定を入れたWebアプリケーションファクトリクラス。
    /// </summary>
    /// <typeparam name="TStartup">Webアプリのスタートアップクラス。</typeparam>
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        #region 静的変数

        /// <summary>
        /// インメモリDBのルート。
        /// </summary>
        public static readonly InMemoryDatabaseRoot InMemoryDatabaseRoot = new InMemoryDatabaseRoot();

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
                    options.UseInMemoryDatabase("TestBlogDB", InMemoryDatabaseRoot);
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
