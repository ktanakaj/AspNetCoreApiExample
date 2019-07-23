// ================================================================================================
// <summary>
//      ブログ記事リポジトリクラスソース</summary>
//
// <copyright file="ArticleRepository.cs">
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
    /// ブログ記事リポジトリクラス。
    /// </summary>
    public class ArticleRepository
    {
        /// <summary>
        /// アプリケーションDBコンテキスト。
        /// </summary>
        private readonly AppDbContext context;

        /// <summary>
        /// コンテキストをDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="context">アプリケーションDBコンテキスト。</param>
        public ArticleRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// ブログ記事を全て取得する。
        /// </summary>
        /// <returns>ブログ記事。</returns>
        public async Task<IList<Article>> FindAll()
        {
            return await this.context.Articles.ToListAsync();
        }

        /// <summary>
        /// ブログ内のブログ記事を取得する。
        /// </summary>
        /// <param name="blogId">ブログID。</param>
        /// <returns>ブログ記事。</returns>
        public async Task<IList<Article>> FindByBlogId(int blogId)
        {
            return await this.context.Articles.Where(a => a.BlogId == blogId).ToListAsync();
        }

        /// <summary>
        /// ブログ記事IDでブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        public Task<Article> Find(int id)
        {
            return this.context.Articles.FindAsync(id);
        }

        /// <summary>
        /// ブログ記事IDでブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
        public async Task<Article> FindOrFail(int id)
        {
            var article = await this.Find(id);
            if (article == null)
            {
                throw new NotFoundException($"id = {id} is not found");
            }

            return article;
        }

        /// <summary>
        /// ブログ記事を保存する。
        /// </summary>
        /// <param name="article">ブログ記事。</param>
        /// <returns>保存したブログ記事。</returns>
        public async Task<Article> Save(Article article)
        {
            if (article.Id > 0)
            {
                this.context.Entry(article).State = EntityState.Modified;
            }
            else
            {
                this.context.Articles.Add(article);
            }

            await this.context.SaveChangesAsync();
            return article;
        }

        /// <summary>
        /// ブログ記事を削除する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>削除したブログ記事。</returns>
        /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
        public async Task<Article> Remove(int id)
        {
            var article = await this.FindOrFail(id);
            this.context.Articles.Remove(article);
            await this.context.SaveChangesAsync();
            return article;
        }
    }
}
