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
    public class TestData
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
                new User()
                {
                    Id = 1, Name = "Taro", Password = "PASSWORD"
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
                    Id = 10, UserId = 1, Name = "Taro's Blog"
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
                    Id = 100, BlogId = 10, Subject = "初めまして", Body = "初めまして、太郎です。ブログにようこそ。"
                },
            };
        }

        #endregion
    }
}
