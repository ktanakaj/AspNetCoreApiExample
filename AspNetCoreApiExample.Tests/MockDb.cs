// ================================================================================================
// <summary>
//      統合テストのモックDBクラスソース</summary>
//
// <copyright file="MockDb.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Honememo.AspNetCoreApiExample.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Honememo.AspNetCoreApiExample.Tests
{
    /// <summary>
    /// モックDBを扱うクラス。
    /// </summary>
    public class MockDb
    {
        #region 定数

        /// <summary>
        /// テストごとにリセットされるインメモリDBのルート。
        /// </summary>
        private readonly InMemoryDatabaseRoot temporaryDatabaseRoot = new InMemoryDatabaseRoot();

        /// <summary>
        /// アプリDBのインメモリDB名。
        /// </summary>
        private readonly string appDbName = "test_app";

        #endregion

        #region アプリ設定メソッド

        /// <summary>
        /// モックDBとテストデータの設定を行う。
        /// </summary>
        /// <param name="services">テスト環境用のサービスコレクション。</param>
        public void ConfigureTestDb(IServiceCollection services)
        {
            // DBをインメモリDBに置き換え
            var dbContextTypes = new Type[]
            {
                typeof(DbContextOptions<AppDbContext>),
            };
            foreach (var t in dbContextTypes)
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == t);
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
            }

            services.AddDbContextPool<AppDbContext>(options => this.ApplyTestDbConfig(options, this.appDbName));

            // 汎用のテストデータを登録
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                appDb.Database.EnsureCreated();
                TestData.InitializeDbForTests(appDb);
            }
        }

        #endregion

        #region テスト補助用メソッド

        // ※ 以下DBコンテキストを取るメソッドが毎回newしているのは、
        //    DI管理下のものを取ると未確定のデータなども取れてしまうため。

        /// <summary>
        /// アプリDBのDBコンテキストを生成する。
        /// </summary>
        /// <returns>DBコンテキスト。</returns>
        public AppDbContext CreateAppDbContext()
        {
            return new AppDbContext(this.ApplyTestDbConfig(new DbContextOptionsBuilder<AppDbContext>(), this.appDbName).Options);
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
            builder.UseInMemoryDatabase(dbname, this.temporaryDatabaseRoot);
            builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            builder.ConfigureWarnings(x => x.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
            return builder;
        }

        #endregion
    }
}
