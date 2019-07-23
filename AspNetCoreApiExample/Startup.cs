// ================================================================================================
// <summary>
//      Webアプリケーション初期設定用クラスソース</summary>
//
// <copyright file="Startup.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;
    using Honememo.AspNetCoreApiExample.Middlewares;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// Webアプリケーション初期設定用のクラスです。
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="configuration">アプリケーション設定。</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// アプリケーション設定。
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// サービスコンテナ登録などの設定用メソッド。
        /// </summary>
        /// <param name="services">サービスコレクション。</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("BlogDB"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<BlogRepository>();
            services.AddScoped<ArticleRepository>();

            // Swagger定義の設定
            services.AddSwaggerGen(c =>
            {
                var asm = Assembly.GetExecutingAssembly();
                var product = asm.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                c.SwaggerDoc("v1", new Info { Title = product.Product, Version = asm.GetName().Version.ToString() });
                var xmlFile = $"{asm.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// HTTPリクエストパイプラインなどの設定用メソッド。
        /// </summary>
        /// <param name="app">アプリケーションビルダー。</param>
        /// <param name="env">ホスト環境。</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Swagger JSONとUIのエンドポイントを有効化
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    var asm = Assembly.GetExecutingAssembly();
                    var product = asm.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                    c.SwaggerEndpoint("v1/swagger.json", product.Product);
                });
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();
        }
    }
}
