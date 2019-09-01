// ================================================================================================
// <summary>
//      ユーザーリポジトリクラスソース</summary>
//
// <copyright file="UserRepository.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;

    /// <summary>
    /// ユーザーリポジトリクラス。
    /// </summary>
    public class UserRepository
    {
        #region メンバー変数

        /// <summary>
        /// ユーザーマネージャー。
        /// </summary>
        private readonly UserManager<User> userManager;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ユーザーマネージャーをDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="userManager">ユーザーマネージャー。</param>
        public UserRepository(UserManager<User> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        #region 参照系メソッド

        /// <summary>
        /// ユーザーを全て取得する。
        /// </summary>
        /// <returns>ユーザー。</returns>
        public async Task<IList<User>> FindAll()
        {
            return await this.userManager.Users.ToListAsync();
        }

        /// <summary>
        /// ユーザーIDでユーザーを取得する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>ユーザー。</returns>
        public Task<User> Find(int id)
        {
            return this.userManager.FindByIdAsync(id.ToString());
        }

        /// <summary>
        /// ユーザー名でユーザーを取得する。
        /// </summary>
        /// <param name="name">ユーザー名。</param>
        /// <returns>ユーザー。</returns>
        public Task<User> FindByName(string name)
        {
            return this.userManager.FindByNameAsync(name);
        }

        /// <summary>
        /// ユーザーIDでユーザーを取得する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>ユーザー。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
        public async Task<User> FindOrFail(int id)
        {
            var user = await this.Find(id);
            if (user == null)
            {
                throw new NotFoundException($"id = {id} is not found");
            }

            return user;
        }

        #endregion

        #region 更新系メソッド

        /// <summary>
        /// ユーザーを登録する。
        /// </summary>
        /// <param name="name">ユーザー名。</param>
        /// <param name="password">パスワード。</param>
        /// <returns>登録したユーザー。</returns>
        /// <remarks>処理の都合上、二回DBを更新します。呼び元でトランザクションしてください。</remarks>
        public async Task<User> CreateBy(string name, string password)
        {
            // ※ 通常は入力値不正しかエラーは発生しない（名前重複やパスワード不許可）
            var user = new User()
            {
                UserName = name,
                LastLogin = DateTimeOffset.UtcNow
            };
            this.ThrowBadRequestExceptionIfResultIsNotSucceeded(
                await this.userManager.CreateAsync(user));
            this.ThrowBadRequestExceptionIfResultIsNotSucceeded(
                await this.userManager.AddPasswordAsync(user, password));
            return user;
        }

        /// <summary>
        /// ユーザーの名前を変更する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <param name="userName">新しいユーザー名。</param>
        /// <returns>更新したユーザー。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
        public async Task<User> ChangeUserName(int id, string userName)
        {
            // ※ 通常は入力値不正しかエラーは発生しない（名前重複）
            var user = await this.FindOrFail(id);
            this.ThrowBadRequestExceptionIfResultIsNotSucceeded(
                await this.userManager.SetUserNameAsync(user, userName));
            return user;
        }

        /// <summary>
        /// ユーザーのパスワードを変更する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <param name="currentPasword">現在のパスワード。</param>
        /// <param name="newPassword">新しいパスワード。</param>
        /// <returns>更新したユーザー。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
        public async Task<User> ChangePassword(int id, string currentPasword, string newPassword)
        {
            // ※ 通常は入力値不正しかエラーは発生しない（パスワード間違いなど）
            var user = await this.FindOrFail(id);
            this.ThrowBadRequestExceptionIfResultIsNotSucceeded(
                await this.userManager.ChangePasswordAsync(user, currentPasword, newPassword));
            return user;
        }

        /// <summary>
        /// ユーザーを更新する。
        /// </summary>
        /// <param name="user">ユーザー。</param>
        /// <returns>更新したユーザー。</returns>
        public async Task<User> Update(User user)
        {
            this.ThrowBadRequestExceptionIfResultIsNotSucceeded(
                await this.userManager.UpdateAsync(user));
            return user;
        }

        /// <summary>
        /// ユーザーを削除する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>削除したユーザー。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
        public async Task<User> Delete(int id)
        {
            var user = await this.FindOrFail(id);
            this.ThrowExceptionIfResultIsNotSucceeded<Exception>(
                await this.userManager.DeleteAsync(user));
            return user;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// UserManagerの戻り値が失敗の場合に例外を投げる。
        /// </summary>
        /// <typeparam name="T">例外クラス。</typeparam>
        /// <param name="result">チェックする戻り値。</param>
        private void ThrowExceptionIfResultIsNotSucceeded<T>(IdentityResult result) where T : Exception, new()
        {
            if (!result.Succeeded)
            {
                throw (T)Activator.CreateInstance(typeof(T), string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        /// <summary>
        /// UserManagerの戻り値が失敗の場合に入力値不正例外を投げる。
        /// </summary>
        /// <param name="result">チェックする戻り値。</param>
        private void ThrowBadRequestExceptionIfResultIsNotSucceeded(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        #endregion
    }
}
