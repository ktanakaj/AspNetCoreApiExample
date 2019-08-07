﻿// ================================================================================================
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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Middlewares;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// Webアプリケーション初期設定用のクラスです。
    /// </summary>
    public class Startup
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="configuration">アプリケーション設定。</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// アプリケーション設定。
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// Webアプリケーションのサービス設定用メソッド。
        /// </summary>
        /// <param name="services">サービスコレクション。</param>
        /// <remarks>設定値の登録や依存関係の登録など、アプリ初期化前の設定を行う。</remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            // DB設定
            services.AddDbContext<AppDbContext>(opt =>
                this.ApplyDbConfig(opt, this.Configuration.GetSection("Database")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // 認証設定
            services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // DI設定
            services.AddScoped<UserRepository>();
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
        /// Webアプリケーションの設定用メソッド。
        /// </summary>
        /// <param name="app">アプリケーションビルダー。</param>
        /// <param name="env">ホスト環境。</param>
        /// <remarks>初期化されたインスタンスなどを元に、アプリ起動前の設定を行う。</remarks>
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

            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// DBオプションビルダーにDB設定値を適用する。
        /// </summary>
        /// <param name="builder">ビルダー。</param>
        /// <param name="dbconf">DB設定値。</param>
        /// <returns>メソッドチェーン用のビルダー。</returns>
        public DbContextOptionsBuilder ApplyDbConfig(DbContextOptionsBuilder builder, IConfigurationSection dbconf)
        {
            switch (dbconf.GetValue<string>("Type")?.ToLower())
            {
                case "mysql":
                    builder.UseMySql(dbconf.GetValue<string>("ConnectionString"));
                    break;
                default:
                    builder.UseInMemoryDatabase("AppDB");
                    break;
            }

            return builder;
        }

        #endregion
    }
}
