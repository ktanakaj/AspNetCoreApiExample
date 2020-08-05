// ================================================================================================
// <summary>
//      統合テストのテストデータクラスソース</summary>
//
// <copyright file="TestData.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Tests
{
    using System.Collections.Generic;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// 統合テストのテストデータ生成クラス。
    /// </summary>
    /// <remarks>
    /// 汎用のテストデータの登録用です。
    /// 個別のテストに特化したデータが必要な場合は、各テスト内で自前で登録してください。
    /// </remarks>
    public static class TestData
    {
        #region 公開メソッド

        /// <summary>
        /// テストデータを登録する。
        /// </summary>
        /// <param name="db">アプリのDBコンテキスト。</param>
        public static void InitializeDbForTests(AppDbContext db)
        {
            db.Users.AddRange(GetUsers());
            db.Blogs.AddRange(GetBlogs());
            db.Articles.AddRange(GetArticles());
            db.SaveChanges();
        }

        #endregion

        #region テストデータ作成メソッド

        /// <summary>
        /// <see cref="User"/>のテストデータを取得する。
        /// </summary>
        /// <returns>テストデータ。</returns>
        private static IList<User> GetUsers()
        {
            return new List<User>()
            {
                // ハッシュ化前のパスワードは全員"PASSWORD"
                new User()
                {
                    Id = 100,
                    UserName = "Taro",
                    NormalizedUserName = "TARO",
                    PasswordHash = "AQAAAAEAACcQAAAAELHYhJQlwFRMpSdYMcf6IKSV1ooi979/BOHd8/wNn07b/K2x4WBjreAz4qErGiEt5w==",
                    SecurityStamp = "7MSORXMIMFO4PO6TD6TSG5UD6ESXO7MU",
                    ConcurrencyStamp = "4f05fd65-7e0a-41d5-80fd-818ee04b86cf",
                },
                new User()
                {
                    Id = 101,
                    UserName = "Jiro",
                    NormalizedUserName = "JIRO",
                    PasswordHash = "AQAAAAEAACcQAAAAEFYWfmdbctdoW3AIDjecLAXTkBx/phWVpTg/YECeHpfF0H1JMN4rg8OsROElIFA4Uw==",
                    SecurityStamp = "LO25XWTLMLC2YZYIHLO4HG7G7MOESIBK",
                    ConcurrencyStamp = "ac1aeab2-d936-4d55-ba06-dd77c4337afc",
                },
            };
        }

        /// <summary>
        /// <see cref="Blog"/>のテストデータを取得する。
        /// </summary>
        /// <returns>テストデータ。</returns>
        private static IList<Blog> GetBlogs()
        {
            return new List<Blog>()
            {
                new Blog()
                {
                    Id = 1000, UserId = 100, Name = "Taro's Blog"
                },
                new Blog()
                {
                    Id = 1010, UserId = 101, Name = "Jiro's Blog"
                },
            };
        }

        /// <summary>
        /// <see cref="Article"/>のテストデータを取得する。
        /// </summary>
        /// <returns>テストデータ。</returns>
        private static IList<Article> GetArticles()
        {
            return new List<Article>()
            {
                new Article()
                {
                    Id = 10000,
                    BlogId = 1000,
                    Subject = "初めまして",
                    Body = "初めまして、太郎です。ブログにようこそ。",
                    Tags = new List<Tag>() { new Tag() { Name = "Blog" }, new Tag() { Name = "お知らせ" } }
                },
                new Article()
                {
                    Id = 10100,
                    BlogId = 1010,
                    Subject = "次郎です",
                    Body = "次郎のブログです。",
                    Tags = new List<Tag>() { new Tag() { Name = "Blog" } }
                },
            };
        }

        #endregion
    }
}
