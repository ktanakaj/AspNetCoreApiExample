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
    using System.Collections.Generic;
    using Xunit;
    using Honememo.AspNetCoreApiExample.Controllers;
    using Honememo.AspNetCoreApiExample.Models;

    /// <summary>
    /// ブログコントローラのテストクラス。
    /// </summary>
    public class BlogsControllerTest
    {
        /// <summary>
        /// GetBlogsのテスト。
        /// </summary>
        [Fact]
        public async void TestGetBlogs()
        {
            // TODO: テストデータを作成してモックコンテキストを渡す。
            //       そもそもインメモリDBなどを使って本物のAPIを呼ぶようなテストにしたい。
            //       https://docs.microsoft.com/ja-jp/aspnet/core/test/integration-tests?view=aspnetcore-2.2
            var controller = new BlogsController(null);
            var result = await controller.GetBlogs();
            var blogs = new List<Blog>(result.Value);
            Assert.NotEmpty(blogs);
        }
    }
}
