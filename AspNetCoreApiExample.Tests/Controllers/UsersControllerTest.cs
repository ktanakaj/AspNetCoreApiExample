// ================================================================================================
// <summary>
//      ユーザーコントローラテストクラスソース</summary>
//
// <copyright file="UsersControllerTest.cs">
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

    /// <summary>
    /// ユーザーコントローラのテストクラス。
    /// </summary>
    public class UsersControllerTest : IClassFixture<CustomWebApplicationFactory>
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
        public UsersControllerTest(CustomWebApplicationFactory factory)
        {
            this.factory = factory;
            this.client = factory.CreateClient();
            (this.authedClient, this.userId) = factory.CreateNewUserClient();
        }

        #endregion

        #region テストメソッド

        /// <summary>
        /// ユーザー一覧を取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetUsers()
        {
            var response = await this.client.GetAsync("/api/users");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JArray>(json);

            // ※ 取れるユーザーは不確定のため、データがあるかのみテスト
            var array = json as JArray;
            Assert.NotEmpty(array);
            Assert.Equal(JTokenType.Integer, array[0]["id"].Type);
            Assert.Equal(JTokenType.String, array[0]["userName"].Type);

            // TODO: 参照不可のユーザー情報が返っていないこと
            // Assert.Null(array[0]["passwordHash"]);
        }

        /// <summary>
        /// 指定されたユーザーを取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetUser()
        {
            var response = await this.client.GetAsync("/api/users/100");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.Equal(100, json["id"]);
            Assert.Equal("Taro", json["userName"]);
        }

        /// <summary>
        /// ユーザーを新規登録のテスト。
        /// </summary>
        [Fact]
        public async void TestCreateUser()
        {
            var body = new UsersController.CreateUserBody() { UserName = "Scott", Password = "Tiger" };
            var response = await this.client.PostAsJsonAsync("/api/users", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.NotNull(json["id"]);
            Assert.Equal(body.UserName, json["userName"]);

            var dbuser = this.factory.CreateDbContext().Users.Find((int)json["id"]);
            Assert.NotNull(dbuser);
            Assert.Equal(body.UserName, dbuser.UserName);
            Assert.NotNull(dbuser.PasswordHash);

            // TODO: パスワードハッシュが正しいものであるかも確認する
            // TODO: 新規ユーザーで認証されたことも確認する
        }

        /// <summary>
        /// ログイン＆ログアウトのテスト。
        /// </summary>
        [Fact]
        public async void TestLoginAndLogout()
        {
            // id=100のユーザーでログインしてログアウトする
            var body = new UsersController.LoginBody() { UserName = "Taro", Password = "PASSWORD" };
            var response = await this.client.PostAsJsonAsync("/api/users/login", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JToken.Parse(responseString);
            Assert.IsType<JObject>(json);

            Assert.NotNull(json["id"]);
            Assert.Equal(body.UserName, json["userName"]);

            response = await this.client.PostAsync("/api/users/logout", null);
            responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);
        }

        /// <summary>
        /// 認証中ユーザーの情報を変更のテスト。
        /// </summary>
        [Fact]
        public async void TestUpdateUser()
        {
            var body = new UsersController.UpdateUserBody() { UserName = "Ken" };
            var response = await this.authedClient.PutAsJsonAsync("/api/users", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbuser = this.factory.CreateDbContext().Users.Find(this.userId);
            Assert.NotNull(dbuser);
            Assert.Equal(body.UserName, dbuser.UserName);
        }

        /// <summary>
        /// 認証中ユーザーのパスワードを変更のテスト。
        /// </summary>
        [Fact]
        public async void TestChangePassword()
        {
            var body = new UsersController.ChangePasswordBody() { CurrentPassword = "TEST_PASSWORD", NewPassword = "KenKen" };
            var response = await this.authedClient.PutAsJsonAsync("/api/users/password", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbuser = this.factory.CreateDbContext().Users.Find(this.userId);
            Assert.NotNull(dbuser);
            Assert.NotNull(dbuser.PasswordHash);

            // TODO: パスワードハッシュが正しいものであるかも確認する
        }

        #endregion
    }
}
