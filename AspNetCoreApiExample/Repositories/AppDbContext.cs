// ================================================================================================
// <summary>
//      アプリケーションDBコンテキストクラスソース</summary>
//
// <copyright file="AppDbContext.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Honememo.AspNetCoreApiExample.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Repositories;

/// <summary>
/// アプリケーションDBコンテキストクラス。
/// </summary>
public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    /// <summary>
    /// コンテキストを生成する。
    /// </summary>
    /// <param name="options">オプション。</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// ブログテーブル。
    /// </summary>
    public DbSet<Blog> Blogs => this.Set<Blog>();

    /// <summary>
    /// ブログ記事テーブル。
    /// </summary>
    public DbSet<Article> Articles => this.Set<Article>();

    /// <summary>
    /// ブログ記事のタグテーブル。
    /// </summary>
    public DbSet<Tag> Tags => this.Set<Tag>();
}