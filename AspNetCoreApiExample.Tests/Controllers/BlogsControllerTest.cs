// ================================================================================================
// <summary>
//      ブログコントローラテストクラスソース</summary>
//
// <copyright file="BlogsControllerTest.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Net.Http.Json;
using Honememo.AspNetCoreApiExample.Dto;
using Honememo.AspNetCoreApiExample.Entities;

namespace Honememo.AspNetCoreApiExample.Tests.Controllers;

/// <summary>
/// ブログコントローラのテストクラス。
/// </summary>
public class BlogsControllerTest : ControllerTestBase
{
    /// <summary>
    /// Webアプリのファクトリーを使用するテストインスタンスを生成する。
    /// </summary>
    /// <param name="factory">Webアプリファクトリー。</param>
    public BlogsControllerTest(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// ブログ一覧を取得のテスト。
    /// </summary>
    /// <returns>処理状態。</returns>
    [Fact]
    public async Task TestGetBlogs()
    {
        var response = await this.Client.GetAsync("/api/blogs", TestContext.Current.CancellationToken);
        await AssertResponse(response);

        var array = await GetResponseBody<IEnumerable<BlogDto>>(response);

        // ※ 取れるブログは不確定のため、データがあるかのみテスト
        Assert.NotNull(array);
        Assert.NotEmpty(array);

        var blog = array.First();
        Assert.True(blog.Id > 0);
        Assert.True(!string.IsNullOrEmpty(blog.Name));
    }

    /// <summary>
    /// 指定されたブログを取得のテスト。
    /// </summary>
    /// <returns>処理状態。</returns>
    [Fact]
    public async Task TestGetBlog()
    {
        var response = await this.Client.GetAsync("/api/blogs/1000", TestContext.Current.CancellationToken);
        await AssertResponse(response);

        var json = await GetResponseBody<BlogDto>(response);
        Assert.NotNull(json);
        Assert.Equal(1000, json.Id);
        Assert.Equal("Taro's Blog", json.Name);
    }

    /// <summary>
    /// ブログを登録のテスト。
    /// </summary>
    /// <returns>処理状態。</returns>
    [Fact]
    public async Task TestPostBlog()
    {
        var now = DateTimeOffset.UtcNow;
        var body = new BlogEditDto() { Name = "New Blog" };
        var response = await this.AuthedClient.PostAsJsonAsync("/api/blogs", body, TestContext.Current.CancellationToken);
        await AssertResponse(response);

        var json = await GetResponseBody<BlogDto>(response);
        Assert.NotNull(json);
        Assert.True(json.Id > 0);
        Assert.Equal(body.Name, json.Name);
        Assert.True(json.CreatedAt > now);
        Assert.True(json.UpdatedAt > now);

        var dbblog = this.Factory.CreateDbContext().Blogs.Find(json.Id);
        Assert.NotNull(dbblog);
        Assert.Equal(body.Name, dbblog.Name);
        Assert.Equal(json.CreatedAt, dbblog.CreatedAt);
        Assert.Equal(json.UpdatedAt, dbblog.UpdatedAt);
    }

    /// <summary>
    /// ブログを更新のテスト。
    /// </summary>
    /// <returns>処理状態。</returns>
    [Fact]
    public async Task TestPutBlog()
    {
        var now = DateTimeOffset.UtcNow;
        var blog = new Blog() { Name = "Blog for PutBlog", UserId = this.UserId };
        var db = this.Factory.CreateDbContext();
        db.Blogs.Add(blog);
        db.SaveChanges();

        var body = new BlogEditDto() { Name = "Updated Blog" };
        var response = await this.AuthedClient.PutAsJsonAsync($"/api/blogs/{blog.Id}", body, TestContext.Current.CancellationToken);
        await AssertResponse(response);

        var dbblog = this.Factory.CreateDbContext().Blogs.Find(blog.Id);
        Assert.NotNull(dbblog);
        Assert.Equal(body.Name, dbblog.Name);
        Assert.True(dbblog.UpdatedAt > now);
    }

    /// <summary>
    /// ブログを削除のテスト。
    /// </summary>
    /// <returns>処理状態。</returns>
    [Fact]
    public async Task TestDeleteBlog()
    {
        var blog = new Blog() { Name = "Blog for DeleteBlog", UserId = this.UserId };
        var db = this.Factory.CreateDbContext();
        db.Blogs.Add(blog);
        db.SaveChanges();

        var response = await this.AuthedClient.DeleteAsync($"/api/blogs/{blog.Id}", TestContext.Current.CancellationToken);
        await AssertResponse(response);

        Assert.Null(this.Factory.CreateDbContext().Blogs.Find(blog.Id));
    }
}
