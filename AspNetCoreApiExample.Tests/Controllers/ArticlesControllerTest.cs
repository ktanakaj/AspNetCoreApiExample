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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Xunit;

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

            var array = JsonConvert.DeserializeObject<IEnumerable<ArticleDto>>(responseString);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            Assert.NotEmpty(array);

            var article = array.First();
            Assert.True(article.Id > 0);
            Assert.True(!string.IsNullOrEmpty(article.Subject));
            Assert.True(!string.IsNullOrEmpty(article.Body));
            Assert.True(article.BlogId > 0);
            Assert.NotNull(article.Tags);

            // 複数のブログを横断して返すこと
            Assert.True(array.Select((a) => a.BlogId).Distinct().Count() > 1);
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

            var array = JsonConvert.DeserializeObject<IEnumerable<ArticleDto>>(responseString);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            Assert.NotEmpty(array);

            var article = array.First();
            Assert.True(article.Id > 0);
            Assert.True(!string.IsNullOrEmpty(article.Subject));
            Assert.True(!string.IsNullOrEmpty(article.Body));
            Assert.True(article.BlogId > 0);
            Assert.NotNull(article.Tags);

            // 指定されたIDのブログのみが取れること
            Assert.Single(array.Select((a) => a.BlogId).Distinct());
        }

        /// <summary>
        /// ブログ記事一覧を取得のテスト（タグ指定）。
        /// </summary>
        [Fact]
        public async void TestGetArticlesByTag()
        {
            var response = await this.client.GetAsync("/api/articles?tag=お知らせ");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var array = JsonConvert.DeserializeObject<IEnumerable<ArticleDto>>(responseString);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            Assert.NotEmpty(array);

            var article = array.First();
            Assert.True(article.Id > 0);
            Assert.True(!string.IsNullOrEmpty(article.Subject));
            Assert.True(!string.IsNullOrEmpty(article.Body));
            Assert.True(article.BlogId > 0);
            Assert.NotEmpty(article.Tags);

            // 指定されたタグを持つブログのみが取れること
            foreach (var a in array)
            {
                Assert.Contains("お知らせ", a.Tags);
            }
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

            var json = JsonConvert.DeserializeObject<ArticleDto>(responseString);
            Assert.Equal(10000, json.Id);
            Assert.Equal("初めまして", json.Subject);
            Assert.Equal("初めまして、太郎です。ブログにようこそ。", json.Body);
            Assert.Contains("Blog", json.Tags);
            Assert.Contains("お知らせ", json.Tags);
        }

        /// <summary>
        /// ブログ記事を登録のテスト。
        /// </summary>
        [Fact]
        public async void TestPostArticle()
        {
            var now = DateTimeOffset.UtcNow;
            var body = new ArticleNewDto() { Subject = "New Article", Body = "New Article Body", BlogId = 1000, Tags = new string[] { "Blog", "新規" } };
            var response = await this.authedClient.PostAsJsonAsync("/api/articles", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JsonConvert.DeserializeObject<ArticleDto>(responseString);
            Assert.True(json.Id > 0);
            Assert.Equal(body.Subject, json.Subject);
            Assert.Equal(body.Body, json.Body);
            Assert.Equal(body.BlogId, json.BlogId);
            Assert.True(json.CreatedAt > now);
            Assert.True(json.UpdatedAt > now);
            Assert.Contains(body.Tags.First(), json.Tags);
            Assert.Contains(body.Tags.Last(), json.Tags);

            var dbarticle = this.factory.CreateDbContext().Articles.Find(json.Id);
            Assert.NotNull(dbarticle);
            Assert.Equal(body.Subject, dbarticle.Subject);
            Assert.Equal(body.Body, dbarticle.Body);
            Assert.Equal(body.BlogId, dbarticle.BlogId);
            Assert.Equal(json.CreatedAt, dbarticle.CreatedAt);
            Assert.Equal(json.UpdatedAt, dbarticle.UpdatedAt);
            foreach (var t in body.Tags)
            {
                Assert.Contains(t, json.Tags);
            }
        }

        /// <summary>
        /// ブログ記事を更新のテスト。
        /// </summary>
        [Fact]
        public async void TestPutArticle()
        {
            var now = DateTimeOffset.UtcNow;
            var article = new Article() { Subject = "Article for PutArticle", Body = "PutArticle Body", BlogId = 1000, Tags = new List<Tag>() { new Tag() { Name = "Blog" }, new Tag() { Name = "変更前" } } };
            var db = this.factory.CreateDbContext();
            db.Articles.Add(article);
            db.SaveChanges();

            var body = new ArticleEditDto() { Subject = "Updated Article", Body = "Updated Article Body", Tags = new string[] { "変更後" } };
            var response = await this.authedClient.PutAsJsonAsync($"/api/articles/{article.Id}", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbarticle = this.factory.CreateDbContext().Articles.Include(a => a.Tags).First(a => a.Id == article.Id);
            Assert.NotNull(dbarticle);
            Assert.Equal(body.Subject, dbarticle.Subject);
            Assert.Equal(body.Body, dbarticle.Body);
            Assert.True(dbarticle.UpdatedAt > now);

            Assert.NotEmpty(dbarticle.Tags.Where(n => n.Name == body.Tags.First()));
            foreach (var o in article.Tags)
            {
                Assert.Empty(dbarticle.Tags.Where(n => n.Name == o.Name));
            }
        }

        /// <summary>
        /// ブログ記事を削除のテスト。
        /// </summary>
        [Fact]
        public async void TestDeleteArticle()
        {
            var article = new Article() { Subject = "Article for DeleteArticle", Body = "DeleteArticle Body", BlogId = 1000, Tags = new List<Tag>() { new Tag() { Name = "Blog" } } };
            var db = this.factory.CreateDbContext();
            db.Articles.Add(article);
            db.SaveChanges();

            var response = await this.authedClient.DeleteAsync($"/api/articles/{article.Id}");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            Assert.Null(this.factory.CreateDbContext().Articles.Find(article.Id));
            Assert.Null(this.factory.CreateDbContext().Tags.Find(article.Id, article.Tags.First().Name));
        }

        #endregion
    }
}
