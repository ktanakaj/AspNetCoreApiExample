// ================================================================================================
// <summary>
//      アプリケーション起動用クラスソース</summary>
//
// <copyright file="Program.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    /// <summary>
    /// アプリケーション起動時に最初に呼ばれるクラスです。
    /// </summary>
    public class Program
    {
        /// <summary>
        /// アプリケーションのメインエントリポイントです。
        /// </summary>
        /// <param name="args">コマンドラインから指定された起動オプション。</param>
        public static void Main(string[] args)
        {
            // ロガーを設定してWebアプリを起動する
            // （SerilogはStartup.csだと設定できないようなので。）
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(ApplyAppConfig(new ConfigurationBuilder()).Build())
                .CreateLogger();
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Webホストビルダーを生成する。
        /// </summary>
        /// <param name="args">コマンドラインから指定された起動オプション。</param>
        /// <returns>生成したWebホストビルダー。</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    ApplyAppConfig(config);
                })
                .UseStartup<Startup>()
                .UseSerilog();

        /// <summary>
        /// 設定ビルダーにアプリ用の設定を適用する。
        /// </summary>
        /// <param name="config">ビルダー。</param>
        /// <returns>メソッドチェーン用のビルダー。</returns>
        private static IConfigurationBuilder ApplyAppConfig(IConfigurationBuilder config)
        {
            return config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json", optional: true)
                .AddEnvironmentVariables(prefix: "EXAMPLEAPP_");
        }
    }
}
