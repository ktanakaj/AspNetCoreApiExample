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
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;

    /// <summary>
    /// ブログリポジトリクラス。
    /// </summary>
    public class BlogRepository
    {
        /// <summary>
        /// アプリケーションDBコンテキスト。
        /// </summary>
        private readonly AppDbContext context;

        /// <summary>
        /// コンテキストをDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="context">アプリケーションDBコンテキスト。</param>
        public BlogRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// ブログを全て取得する。
        /// </summary>
        /// <returns>ブログ。</returns>
        public async Task<IList<Blog>> FindAll()
        {
            return await this.context.Blogs.ToListAsync();
        }

        /// <summary>
        /// ブログIDでブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        public Task<Blog> Find(int id)
        {
            return this.context.Blogs.FindAsync(id);
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
                throw new NotFoundException($"id = {id} is not found");
            }

            return blog;
        }

        /// <summary>
        /// ブログを保存する。
        /// </summary>
        /// <param name="blog">ブログ。</param>
        /// <returns>保存したブログ。</returns>
        public async Task<Blog> Save(Blog blog)
        {
            if (blog.Id > 0)
            {
                this.context.Entry(blog).State = EntityState.Modified;
            }
            else
            {
                this.context.Blogs.Add(blog);
            }

            await this.context.SaveChangesAsync();
            return blog;
        }

        /// <summary>
        /// ブログを削除する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>削除したブログ。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        public async Task<Blog> Remove(int id)
        {
            var blog = await this.FindOrFail(id);
            this.context.Blogs.Remove(blog);
            await this.context.SaveChangesAsync();
            return blog;
        }
    }
}
