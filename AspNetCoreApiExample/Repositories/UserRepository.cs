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
        /// <param name="user">ユーザー。</param>
        /// <returns>登録したユーザー。</returns>
        public async Task<User> Create(User user)
        {
            user.Id = 0;
            var result = await this.userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }

            return user;
        }

        /// <summary>
        /// ユーザーを更新する。
        /// </summary>
        /// <param name="user">ユーザー。</param>
        /// <returns>更新したユーザー。</returns>
        public async Task<User> Update(User user)
        {
            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }

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
            var result = await this.userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }

            return user;
        }

        #endregion
    }
}
