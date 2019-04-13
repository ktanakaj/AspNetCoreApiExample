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
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// アプリケーション起動時に最初に呼ばれるクラスです。
    /// </summary>
    public class Program
    {
        /// <summary>
        /// アプリケーションのメインエントリポイントです。
        /// </summary>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
