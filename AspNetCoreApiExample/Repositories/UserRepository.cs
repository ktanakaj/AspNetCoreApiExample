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
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// ユーザーリポジトリクラス。
    /// </summary>
    public class UserRepository
    {
        #region メンバー変数

        /// <summary>
        /// アプリケーションDBコンテキスト。
        /// </summary>
        private readonly AppDbContext context;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンテキストを使用するリポジトリを生成する。
        /// </summary>
        /// <param name="context">アプリケーションDBコンテキスト。</param>
        public UserRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region 参照系メソッド

        /// <summary>
        /// ユーザーを全て取得する。
        /// </summary>
        /// <returns>ユーザー。</returns>
        public async Task<IList<User>> FindAll()
        {
            return await this.context.Users.OrderBy(b => b.UserName).ThenBy(b => b.Id).ToListAsync();
        }

        /// <summary>
        /// ユーザーIDでユーザーを取得する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>ユーザー。</returns>
        public async Task<User> Find(int id)
        {
            return await this.context.Users.FindAsync(id);
        }

        /// <summary>
        /// ユーザー名でユーザーを取得する。
        /// </summary>
        /// <param name="name">ユーザー名。</param>
        /// <returns>ユーザー。取得できない場合null。</returns>
        public async Task<User> FindByName(string name)
        {
            return await this.context.Users.FirstOrDefaultAsync(u => u.UserName == name);
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
                throw new NotFoundException($"id={id} is not found");
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
            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// ユーザーを更新する。
        /// </summary>
        /// <param name="user">ユーザー。</param>
        /// <returns>更新したユーザー。</returns>
        public async Task<User> Update(User user)
        {
            this.context.Entry(user).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
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
            this.context.Users.Remove(user);
            await this.context.SaveChangesAsync();
            return user;
        }

        #endregion
    }
}
