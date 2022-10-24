// ================================================================================================
// <summary>
//      ブログ記事コントローラテストクラスソース</summary>
//
// <copyright file="ArticlesControllerTest.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Net.Http.Json;
using Honememo.AspNetCoreApiExample.Dto;
using Honememo.AspNetCoreApiExample.Entities;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Tests.Controllers
{
    /// <summary>
    /// ブログ記事コントローラのテストクラス。
    /// </summary>
    public class ArticlesControllerTest : ControllerTestBase
    {
        #region コンストラクタ

        /// <summary>
        /// Webアプリのファクトリーを使用するテストインスタンスを生成する。
        /// </summary>
        /// <param name="factory">Webアプリファクトリー。</param>
        public ArticlesControllerTest(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        #endregion

        #region テストメソッド

        /// <summary>
        /// ブログ記事一覧を取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetArticles()
        {
            var response = await this.Client.GetAsync("/api/articles");
            await AssertResponse(response);

            var array = await GetResponseBody<IEnumerable<ArticleDto>>(response);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            Assert.NotNull(array);
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
            var response = await this.Client.GetAsync("/api/articles?blogId=1000");
            await AssertResponse(response);

            var array = await GetResponseBody<IEnumerable<ArticleDto>>(response);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            Assert.NotNull(array);
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
            var response = await this.Client.GetAsync("/api/articles?tag=お知らせ");
            await AssertResponse(response);

            var array = await GetResponseBody<IEnumerable<ArticleDto>>(response);

            // ※ 取れるブログ記事は不確定のため、データがあるかのみテスト
            Assert.NotNull(array);
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
            var response = await this.Client.GetAsync("/api/articles/10000");
            await AssertResponse(response);

            var json = await GetResponseBody<ArticleDto>(response);
            Assert.NotNull(json);
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
            var response = await this.AuthedClient.PostAsJsonAsync("/api/articles", body);
            await AssertResponse(response);

            var json = await GetResponseBody<ArticleDto>(response);
            Assert.NotNull(json);
            Assert.True(json.Id > 0);
            Assert.Equal(body.Subject, json.Subject);
            Assert.Equal(body.Body, json.Body);
            Assert.Equal(body.BlogId, json.BlogId);
            Assert.True(json.CreatedAt > now);
            Assert.True(json.UpdatedAt > now);
            Assert.Contains(body.Tags.First(), json.Tags);
            Assert.Contains(body.Tags.Last(), json.Tags);

            var dbarticle = this.Factory.CreateDbContext().Articles.Find(json.Id);
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
            var db = this.Factory.CreateDbContext();
            db.Articles.Add(article);
            db.SaveChanges();

            var body = new ArticleEditDto() { Subject = "Updated Article", Body = "Updated Article Body", Tags = new string[] { "変更後" } };
            var response = await this.AuthedClient.PutAsJsonAsync($"/api/articles/{article.Id}", body);
            await AssertResponse(response);

            var dbarticle = this.Factory.CreateDbContext().Articles.Include(a => a.Tags).First(a => a.Id == article.Id);
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
            var db = this.Factory.CreateDbContext();
            db.Articles.Add(article);
            db.SaveChanges();

            var response = await this.AuthedClient.DeleteAsync($"/api/articles/{article.Id}");
            await AssertResponse(response);

            Assert.Null(this.Factory.CreateDbContext().Articles.Find(article.Id));
            Assert.Null(this.Factory.CreateDbContext().Tags.Find(article.Id, article.Tags.First().Name));
        }

        #endregion
    }
}
