// ================================================================================================
// <summary>
//      ブログコントローラテストクラスソース</summary>
//
// <copyright file="BlogsControllerTest.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// ブログコントローラのテストクラス。
    /// </summary>
    public class BlogsControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        #region メンバー変数

        /// <summary>
        /// Webアプリのファクトリー。
        /// </summary>
        private readonly CustomWebApplicationFactory factory;

        /// <summary>
        /// Webアプリテスト用のHTTPクライアント。
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// Webアプリテスト用の認証済HTTPクライアント。
        /// </summary>
        private readonly HttpClient authedClient;

        /// <summary>
        /// 認証済HTTPクライアントのユーザーID。
        /// </summary>
        private readonly int userId;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// WebアプリのファクトリーをDIしてテストインスタンスを生成する。
        /// </summary>
        /// <param name="factory">Webアプリファクトリー。</param>
        public BlogsControllerTest(CustomWebApplicationFactory factory)
        {
            this.factory = factory;
            this.client = factory.CreateClient();
            (this.authedClient, this.userId) = factory.CreateAuthedClient();
        }

        #endregion

        #region テストメソッド

        /// <summary>
        /// ブログ一覧を取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetBlogs()
        {
            var response = await this.client.GetAsync("/api/blogs");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var array = JsonConvert.DeserializeObject<IEnumerable<BlogDto>>(responseString);

            // ※ 取れるブログは不確定のため、データがあるかのみテスト
            Assert.NotEmpty(array);

            var blog = array.First();
            Assert.True(blog.Id > 0);
            Assert.True(!string.IsNullOrEmpty(blog.Name));
        }

        /// <summary>
        /// 指定されたブログを取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetBlog()
        {
            var response = await this.client.GetAsync("/api/blogs/1000");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JsonConvert.DeserializeObject<BlogDto>(responseString);
            Assert.Equal(1000, json.Id);
            Assert.Equal("Taro's Blog", json.Name);
        }

        /// <summary>
        /// ブログを登録のテスト。
        /// </summary>
        [Fact]
        public async void TestPostBlog()
        {
            var now = DateTimeOffset.UtcNow;
            var body = new BlogEditDto() { Name = "New Blog" };
            var response = await this.authedClient.PostAsJsonAsync("/api/blogs", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JsonConvert.DeserializeObject<BlogDto>(responseString);
            Assert.True(json.Id > 0);
            Assert.Equal(body.Name, json.Name);
            Assert.True(json.CreatedAt > now);
            Assert.True(json.UpdatedAt > now);

            var dbblog = this.factory.CreateDbContext().Blogs.Find(json.Id);
            Assert.NotNull(dbblog);
            Assert.Equal(body.Name, dbblog.Name);
            Assert.Equal(json.CreatedAt, dbblog.CreatedAt);
            Assert.Equal(json.UpdatedAt, dbblog.UpdatedAt);
        }

        /// <summary>
        /// ブログを更新のテスト。
        /// </summary>
        [Fact]
        public async void TestPutBlog()
        {
            var now = DateTimeOffset.UtcNow;
            var blog = new Blog() { Name = "Blog for PutBlog", UserId = this.userId };
            var db = this.factory.CreateDbContext();
            db.Blogs.Add(blog);
            db.SaveChanges();

            var body = new BlogEditDto() { Name = "Updated Blog" };
            var response = await this.authedClient.PutAsJsonAsync($"/api/blogs/{blog.Id}", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbblog = this.factory.CreateDbContext().Blogs.Find(blog.Id);
            Assert.NotNull(dbblog);
            Assert.Equal(body.Name, dbblog.Name);
            Assert.True(dbblog.UpdatedAt > now);
        }

        /// <summary>
        /// ブログを削除のテスト。
        /// </summary>
        [Fact]
        public async void TestDeleteBlog()
        {
            var blog = new Blog() { Name = "Blog for DeleteBlog", UserId = this.userId };
            var db = this.factory.CreateDbContext();
            db.Blogs.Add(blog);
            db.SaveChanges();

            var response = await this.authedClient.DeleteAsync($"/api/blogs/{blog.Id}");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            Assert.Null(this.factory.CreateDbContext().Blogs.Find(blog.Id));
        }

        #endregion
    }
}
