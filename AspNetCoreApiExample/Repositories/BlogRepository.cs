// ================================================================================================
// <summary>
//      ブログリポジトリクラスソース</summary>
//
// <copyright file="BlogRepository.cs">
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
    using Microsoft.EntityFrameworkCore;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;

    /// <summary>
    /// ブログリポジトリクラス。
    /// </summary>
    public class BlogRepository
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
        public BlogRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region 参照系メソッド

        /// <summary>
        /// ブログを全て取得する。
        /// </summary>
        /// <returns>ブログ。</returns>
        public async Task<IList<Blog>> FindAll()
        {
            return await this.context.Blogs.OrderBy(b => b.Name).ThenBy(b => b.UserId).ToListAsync();
        }

        /// <summary>
        /// ブログIDでブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        public async Task<Blog> Find(int id)
        {
            return await this.context.Blogs.FindAsync(id);
        }

        /// <summary>
        /// ブログIDでブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        public async Task<Blog> FindOrFail(int id)
        {
            var blog = await this.Find(id);
            if (blog == null)
            {
                throw new NotFoundException($"id={id} is not found");
            }

            return blog;
        }

        #endregion

        #region 更新系メソッド

        /// <summary>
        /// ブログを登録する。
        /// </summary>
        /// <param name="blog">ブログ。</param>
        /// <returns>登録したブログ。</returns>
        public async Task<Blog> Create(Blog blog)
        {
            blog.Id = 0;
            this.context.Blogs.Add(blog);
            await this.context.SaveChangesAsync();
            return blog;
        }

        /// <summary>
        /// ブログを更新する。
        /// </summary>
        /// <param name="blog">ブログ。</param>
        /// <returns>更新したブログ。</returns>
        public async Task<Blog> Update(Blog blog)
        {
            this.context.Entry(blog).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
            return blog;
        }

        /// <summary>
        /// ブログを削除する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>削除したブログ。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        public async Task<Blog> Delete(int id)
        {
            var blog = await this.FindOrFail(id);
            this.context.Blogs.Remove(blog);
            await this.context.SaveChangesAsync();
            return blog;
        }

        #endregion
    }
}
