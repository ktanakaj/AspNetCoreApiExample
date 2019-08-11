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
    using System.Net.Http;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using Honememo.AspNetCoreApiExample.Controllers;
    using Honememo.AspNetCoreApiExample.Entities;

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

            var json = JToken.Parse(responseString);
            Assert.IsType<JArray>(json);

            // ※ 取れるブログは不確定のため、データがあるかのみテスト
            var array = json as JArray;
            Assert.NotEmpty(array);
            Assert.Equal(JTokenType.Integer, array[0]["id"].Type);
            Assert.Equal(JTokenType.String, array[0]["name"].Type);
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

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.Equal(1000, json["id"]);
            Assert.Equal("Taro's Blog", json["name"]);
        }

        /// <summary>
        /// ブログを登録のテスト。
        /// </summary>
        [Fact]
        public async void TestPostBlog()
        {
            var body = new BlogsController.BlogBody() { Name = "New Blog" };
            var response = await this.authedClient.PostAsJsonAsync("/api/blogs", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.NotNull(json["id"]);
            Assert.Equal(body.Name, json["name"]);

            var dbblog = this.factory.CreateDbContext().Blogs.Find((int)json["id"]);
            Assert.NotNull(dbblog);
            Assert.Equal(body.Name, dbblog.Name);
        }

        /// <summary>
        /// ブログを更新のテスト。
        /// </summary>
        [Fact]
        public async void TestPutBlog()
        {
            var blog = new Blog() { Id = 2001, Name = "Blog for PutBlog", UserId = this.userId };
            var db = this.factory.CreateDbContext();
            db.Blogs.Add(blog);
            db.SaveChanges();

            var body = new BlogsController.BlogBody() { Name = "Updated Blog" };
            var response = await this.authedClient.PutAsJsonAsync($"/api/blogs/{blog.Id}", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbblog = this.factory.CreateDbContext().Blogs.Find(blog.Id);
            Assert.NotNull(dbblog);
            Assert.Equal(body.Name, dbblog.Name);
        }

        /// <summary>
        /// ブログを削除のテスト。
        /// </summary>
        [Fact]
        public async void TestDeleteBlog()
        {
            var blog = new Blog() { Id = 2002, Name = "Blog for DeleteBlog", UserId = this.userId };
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
