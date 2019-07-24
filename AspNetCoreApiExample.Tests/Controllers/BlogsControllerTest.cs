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
    using Xunit;
    using Newtonsoft.Json.Linq;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// ブログコントローラのテストクラス。
    /// </summary>
    public class BlogsControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        #region メンバー変数

        /// <summary>
        /// Webアプリテスト用のHTTPクライアント。
        /// </summary>
        private readonly HttpClient client;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// WebアプリのファクトリクラスをDIしてテストインスタンスを生成する。
        /// </summary>
        /// <param name="factory">Webアプリファクトリクラス。</param>
        public BlogsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            this.client = factory.CreateClient();
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
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var json = JToken.Parse(responseString);
            Assert.IsType<JArray>(json);

            var array = json as JArray;
            Assert.NotEmpty(array);
            Assert.Equal(10, array[0]["id"]);
            Assert.Equal("Taro's Blog", array[0]["name"]);
        }

        /// <summary>
        /// 指定されたブログを取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetBlog()
        {
            var response = await this.client.GetAsync("/api/blogs/10");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.Equal(10, json["id"]);
            Assert.Equal("Taro's Blog", json["name"]);
        }

        /// <summary>
        /// ブログを登録のテスト。
        /// </summary>
        [Fact]
        public async void TestPostBlog()
        {
            var blog = new Blog() { Name = "New Blog" };
            var response = await this.client.PostAsJsonAsync("/api/blogs", blog);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.NotNull(json["id"]);
            Assert.Equal("New Blog", json["name"]);
        }

        #endregion
    }
}
