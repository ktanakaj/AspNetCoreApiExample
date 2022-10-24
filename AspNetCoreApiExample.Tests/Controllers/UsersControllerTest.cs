// ================================================================================================
// <summary>
//      ユーザーコントローラテストクラスソース</summary>
//
// <copyright file="UsersControllerTest.cs">
//      Copyright (C) 2022 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.Net.Http.Json;
using Honememo.AspNetCoreApiExample.Dto;

namespace Honememo.AspNetCoreApiExample.Tests.Controllers
{
    /// <summary>
    /// ユーザーコントローラのテストクラス。
    /// </summary>
    public class UsersControllerTest : ControllerTestBase
    {
        #region コンストラクタ

        /// <summary>
        /// Webアプリのファクトリーを使用するテストインスタンスを生成する。
        /// </summary>
        /// <param name="factory">Webアプリファクトリー。</param>
        public UsersControllerTest(CustomWebApplicationFactory factory)
            : base(factory)
        {
            // ユーザー情報を書き換えるテストがあるので、新規ユーザーのクライアントを使用
            (this.AuthedClient, this.UserId) = factory.CreateNewUserClient();
        }

        #endregion

        #region テストメソッド

        /// <summary>
        /// ユーザー一覧を取得のテスト。
        /// </summary>
        [Fact]
        public async void TestGetUsers()
        {
            var response = await this.Client.GetAsync("/api/users");
            await AssertResponse(response);

            var array = await GetResponseBody<IEnumerable<UserDto>>(response);

            // ※ 取れるユーザーは不確定のため、データがあるかのみテスト
            Assert.NotNull(array);
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
            var response = await this.Client.GetAsync("/api/users/100");
            await AssertResponse(response);

            var json = await GetResponseBody<UserDto>(response);
            Assert.NotNull(json);
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
            var response = await this.Client.PostAsJsonAsync("/api/users", body);
            await AssertResponse(response);

            var json = await GetResponseBody<UserDto>(response);
            Assert.NotNull(json);
            Assert.True(json.Id > 0);
            Assert.Equal(body.UserName, json.UserName);
            Assert.True(json.LastLogin > now);
            Assert.True(json.CreatedAt > now);
            Assert.True(json.UpdatedAt > now);

            var dbuser = this.Factory.CreateDbContext().Users.Find(json.Id);
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
            var response = await this.Client.PostAsJsonAsync("/api/users/login", body);
            await AssertResponse(response);

            var json = await GetResponseBody<UserDto>(response);
            Assert.NotNull(json);
            Assert.Equal(100, json.Id);
            Assert.Equal(body.UserName, json.UserName);
            Assert.True(json.LastLogin > now);

            var dbuser = this.Factory.CreateDbContext().Users.Find(json.Id);
            Assert.NotNull(dbuser);
            Assert.Equal(json.LastLogin, dbuser.LastLogin);

            response = await this.Client.PostAsync("/api/users/logout", null);
            await AssertResponse(response);
        }

        /// <summary>
        /// 認証中ユーザーの情報を変更のテスト。
        /// </summary>
        [Fact]
        public async void TestUpdateUser()
        {
            var now = DateTimeOffset.UtcNow;
            var body = new UserEditDto() { UserName = "Ken" };
            var response = await this.AuthedClient.PutAsJsonAsync("/api/users", body);
            await AssertResponse(response);

            var dbuser = this.Factory.CreateDbContext().Users.Find(this.UserId);
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
            var response = await this.AuthedClient.PutAsJsonAsync("/api/users/password", body);
            await AssertResponse(response);

            var dbuser = this.Factory.CreateDbContext().Users.Find(this.UserId);
            Assert.NotNull(dbuser);
            Assert.NotNull(dbuser.PasswordHash);
            Assert.True(dbuser.UpdatedAt > now);

            // TODO: パスワードハッシュが正しいものであるかも確認する
        }

        #endregion
    }
}
