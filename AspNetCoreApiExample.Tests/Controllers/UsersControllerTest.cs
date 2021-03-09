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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Honememo.AspNetCoreApiExample.Dto;
    using Newtonsoft.Json;
    using Xunit;

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

            var array = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(responseString);

            // ※ 取れるユーザーは不確定のため、データがあるかのみテスト
            Assert.NotEmpty(array);

            var user = array.First();
            Assert.True(user.Id > 0);
            Assert.True(!string.IsNullOrEmpty(user.UserName));
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

            var json = JsonConvert.DeserializeObject<UserDto>(responseString);
            Assert.Equal(100, json.Id);
            Assert.Equal("Taro", json.UserName);
        }

        /// <summary>
        /// ユーザーを新規登録のテスト。
        /// </summary>
        [Fact]
        public async void TestCreateUser()
        {
            var now = DateTimeOffset.UtcNow;
            var body = new UserNewDto() { UserName = "Scott", Password = "Tiger" };
            var response = await this.client.PostAsJsonAsync("/api/users", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JsonConvert.DeserializeObject<UserDto>(responseString);
            Assert.True(json.Id > 0);
            Assert.Equal(body.UserName, json.UserName);
            Assert.True(json.LastLogin > now);
            Assert.True(json.CreatedAt > now);
            Assert.True(json.UpdatedAt > now);

            var dbuser = this.factory.CreateDbContext().Users.Find(json.Id);
            Assert.NotNull(dbuser);
            Assert.Equal(body.UserName, dbuser.UserName);
            Assert.NotNull(dbuser.PasswordHash);
            Assert.Equal(json.LastLogin, dbuser.LastLogin);
            Assert.Equal(json.CreatedAt, dbuser.CreatedAt);
            Assert.Equal(json.UpdatedAt, dbuser.UpdatedAt);

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
            var now = DateTimeOffset.UtcNow;
            var body = new LoginDto() { UserName = "Taro", Password = "PASSWORD" };
            var response = await this.client.PostAsJsonAsync("/api/users/login", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var json = JsonConvert.DeserializeObject<UserDto>(responseString);
            Assert.Equal(100, json.Id);
            Assert.Equal(body.UserName, json.UserName);
            Assert.True(json.LastLogin > now);

            var dbuser = this.factory.CreateDbContext().Users.Find(json.Id);
            Assert.Equal(json.LastLogin, dbuser.LastLogin);

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
            var now = DateTimeOffset.UtcNow;
            var body = new UserEditDto() { UserName = "Ken" };
            var response = await this.authedClient.PutAsJsonAsync("/api/users", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbuser = this.factory.CreateDbContext().Users.Find(this.userId);
            Assert.NotNull(dbuser);
            Assert.Equal(body.UserName, dbuser.UserName);
            Assert.True(dbuser.UpdatedAt > now);
        }

        /// <summary>
        /// 認証中ユーザーのパスワードを変更のテスト。
        /// </summary>
        [Fact]
        public async void TestChangePassword()
        {
            var now = DateTimeOffset.UtcNow;
            var body = new ChangePasswordDto() { CurrentPassword = "TEST_PASSWORD", NewPassword = "KenKen" };
            var response = await this.authedClient.PutAsJsonAsync("/api/users/password", body);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, responseString);

            var dbuser = this.factory.CreateDbContext().Users.Find(this.userId);
            Assert.NotNull(dbuser);
            Assert.NotNull(dbuser.PasswordHash);
            Assert.True(dbuser.UpdatedAt > now);

            // TODO: パスワードハッシュが正しいものであるかも確認する
        }

        #endregion
    }
}
