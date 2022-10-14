// ================================================================================================
// <summary>
//      アプリケーション起動用クラスソース</summary>
//
// <copyright file="Program.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Net;
using System.Reflection;
using AutoMapper;
using Hellang.Middleware.ProblemDetails;
using Honememo.AspNetCoreApiExample.Dto;
using Honememo.AspNetCoreApiExample.Entities;
using Honememo.AspNetCoreApiExample.Exceptions;
using Honememo.AspNetCoreApiExample.Middlewares;
using Honememo.AspNetCoreApiExample.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;

// ThreadPoolのワーカー数が少ないと、一気に負荷が掛かった場合に増えるまで処理が詰まってしまうので、大幅に増やす
ThreadPool.GetMinThreads(out _, out int minCompletionPortThread);
ThreadPool.SetMinThreads(300, minCompletionPortThread);

// Webアプリを初期化する
var builder = WebApplication.CreateBuilder(args);

// 設定ファイルの参照追加
ApplyAppConfig(builder.Configuration);

// マッピング設定
var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
builder.Services.AddSingleton(mappingConfig.CreateMapper());

// DB設定
builder.Services.AddDbContextPool<AppDbContext>((provider, options) =>
{
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory(provider.GetService<ILoggerFactory>());
    ApplyDbConfig(options, builder.Configuration.GetSection("Database"));
});

// コントローラの設定
builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
{
    options.MapToStatusCode<BadRequestException>((int)HttpStatusCode.BadRequest);
    options.MapToStatusCode<ForbiddenException>((int)HttpStatusCode.Forbidden);
    options.MapToStatusCode<NotFoundException>((int)HttpStatusCode.NotFound);
});

// 認証設定
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options => builder.Configuration.Bind("Identity", options));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden);
    options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized);
});

// リポジトリやサービスのDI設定
builder.Services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<AppDbContext>());
builder.Services.Scan(scan => scan
    .FromCallingAssembly()
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository") || type.Name.EndsWith("Service")))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

// Swagger定義の設定
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var asm = Assembly.GetExecutingAssembly();
    var product = asm.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
    c.SwaggerDoc("v1", new OpenApiInfo { Title = product?.Product, Version = asm.GetName()?.Version?.ToString() });
    var xmlFile = $"{asm.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// 初期化したWebアプリにルートなどの設定を行う
var app = builder.Build();

// 開発用ページの設定
if (app.Environment.IsDevelopment())
{
    // Swagger JSONとUIのエンドポイントを有効化
    app.UseSwagger();
    app.UseSwaggerUI();
}

// エラーレスポンスの設定
app.UseProblemDetails();

// ミドルウェアの設定
app.UseMiddleware<EnableBufferingMiddleware>();
app.UseMiddleware<AccessLogMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// エンドポイントの設定
app.MapControllers();

// Webアプリを起動する
app.Run();

/// <summary>
/// アプリケーション起動時に最初に呼ばれるクラス。
/// </summary>
/// <remarks>
/// .NET 6からProgramクラスは存在するものの定義不要になっているが、
/// 既存のメソッドがいくつかあったのと、それだとテストプロジェクトから参照できないので、
/// 部分クラス宣言をしてpublicにもしている。
/// </remarks>
/// <see href="https://learn.microsoft.com/ja-jp/aspnet/core/test/integration-tests?view=aspnetcore-6.0#basic-tests-with-the-default-webapplicationfactory"/>
public partial class Program
{
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

    /// <summary>
    /// DBオプションビルダーにDB設定値を適用する。
    /// </summary>
    /// <param name="builder">ビルダー。</param>
    /// <param name="dbconf">DB設定値。</param>
    /// <returns>メソッドチェーン用のビルダー。</returns>
    private static DbContextOptionsBuilder ApplyDbConfig(DbContextOptionsBuilder builder, IConfigurationSection dbconf)
    {
        // DB接続設定
        switch (dbconf.GetValue<string>("Type")?.ToLower())
        {
            case "mysql":
                builder.UseMySql(dbconf.GetValue<string>("ConnectionString"), ServerVersion.Parse("8.0.28-mysql"));
                break;
            default:
                builder.UseInMemoryDatabase("AppDB");
                builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                break;
        }

        return builder;
    }

    /// <summary>
    /// 認証のリダイレクトをHTTPステータスコードに差し替える。
    /// </summary>
    /// <param name="statusCode">返すHTTPステータスコード。</param>
    /// <returns>差し替え用のファンクション。</returns>
    private static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode)
    {
        return context =>
        {
            context.Response.StatusCode = (int)statusCode;
            return Task.CompletedTask;
        };
    }
}
