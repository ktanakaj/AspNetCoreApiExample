// ================================================================================================
// <summary>
//      ブログ記事コントローラテストクラスソース</summary>
//
// <copyright file="ArticlesControllerTest.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Tests.Controllers
{
    using System.Linq;
    using System.Net.Http;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using Honememo.AspNetCoreApiExample.Controllers;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// ブログ記事コントローラのテストクラス。
    /// </summary>
    public class ArticlesControllerTest : IClassFixture<CustomWebApplicationFactory>
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
        public ArticlesControllerTest(CustomWebApplicationFactory factory)
        {
            this.factory = factory;
            this.client = factory.CreateClient();
            (this.authedClient, this.userId) = factory.CreateAuthedClient();
        }

        #endregion

        #region テストメソッド

        /// <summary>
        /// ブログ記事一覧を取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetArticles()
        {
            var response = await this.client.GetAsync("/api/articles");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JArray>(json);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            var array = json as JArray;
            Assert.NotEmpty(array);
            Assert.Equal(JTokenType.Integer, array[0]["id"].Type);
            Assert.Equal(JTokenType.String, array[0]["subject"].Type);
            Assert.Equal(JTokenType.String, array[0]["body"].Type);
            Assert.Equal(JTokenType.Integer, array[0]["blogId"].Type);

            // 複数のブログを横断して返すこと
            Assert.True(array.Select((a) => a["blogId"]).Distinct().Count() > 1);
        }

        /// <summary>
        /// ブログ記事一覧を取得のテスト（ブログID指定）。
        /// </summary>
        [Fact]
        public async void TestGetArticlesByBlogId()
        {
            var response = await this.client.GetAsync("/api/articles?blogId=1000");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JArray>(json);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            var array = json as JArray;
            Assert.NotEmpty(array);
            Assert.Equal(JTokenType.Integer, array[0]["id"].Type);
            Assert.Equal(JTokenType.String, array[0]["subject"].Type);
            Assert.Equal(JTokenType.String, array[0]["body"].Type);
            Assert.Equal(1000, array[0]["blogId"]);

            // 指定されたIDのブログのみが取れること
            Assert.Single(array.Select((a) => a["blogId"]).Distinct());
        }

        /// <summary>
        /// 指定されたブログ記事を取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetArticle()
        {
            var response = await this.client.GetAsync("/api/articles/10000");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.Equal(10000, json["id"]);
            Assert.Equal("初めまして", json["subject"]);
            Assert.Equal("初めまして、太郎です。ブログにようこそ。", json["body"]);
        }

        /// <summary>
        /// ブログ記事を登録のテスト。
        /// </summary>
        [Fact]
        public async void TestPostArticle()
        {
            var body = new ArticlesController.ArticleBody() { Subject = "New Article", Body = "New Article Body", BlogId = 1000 };
            var response = await this.authedClient.PostAsJsonAsync("/api/articles", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.NotNull(json["id"]);
            Assert.Equal(body.Subject, json["subject"]);
            Assert.Equal(body.Body, json["body"]);
            Assert.Equal(body.BlogId, json["blogId"]);

            var dbarticle = this.factory.CreateDbContext().Articles.Find((int)json["id"]);
            Assert.NotNull(dbarticle);
            Assert.Equal(body.Subject, dbarticle.Subject);
            Assert.Equal(body.Body, dbarticle.Body);
            Assert.Equal(body.BlogId, dbarticle.BlogId);
        }

        /// <summary>
        /// ブログ記事を更新のテスト。
        /// </summary>
        [Fact]
        public async void TestPutArticle()
        {
            var article = new Article() { Id = 20001, Subject = "Article for PutArticle", Body = "PutArticle Body", BlogId = 1000 };
            var db = this.factory.CreateDbContext();
            db.Articles.Add(article);
            db.SaveChanges();

            var body = new ArticlesController.ArticleBody() { Subject = "Updated Article", Body = "Updated Article Body" };
            var response = await this.authedClient.PutAsJsonAsync($"/api/articles/{article.Id}", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbarticle = this.factory.CreateDbContext().Articles.Find(article.Id);
            Assert.NotNull(dbarticle);
            Assert.Equal(body.Subject, dbarticle.Subject);
            Assert.Equal(body.Body, dbarticle.Body);
        }

        /// <summary>
        /// ブログ記事を削除のテスト。
        /// </summary>
        [Fact]
        public async void TestDeleteArticle()
        {
            var article = new Article() { Id = 20002, Subject = "Article for DeleteArticle", Body = "DeleteArticle Body", BlogId = 1000 };
            var db = this.factory.CreateDbContext();
            db.Articles.Add(article);
            db.SaveChanges();

            var response = await this.authedClient.DeleteAsync($"/api/articles/{article.Id}");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            Assert.Null(this.factory.CreateDbContext().Articles.Find(article.Id));
        }

        #endregion
    }
}
