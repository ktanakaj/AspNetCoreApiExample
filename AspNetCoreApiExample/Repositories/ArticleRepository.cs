﻿// ================================================================================================
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
        #region メンバー変数

        /// <summary>
        /// アプリケーションDBコンテキスト。
        /// </summary>
        private readonly AppDbContext context;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンテキストをDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="context">アプリケーションDBコンテキスト。</param>
        public ArticleRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region 参照系メソッド

        /// <summary>
        /// ブログ記事を全て取得する。
        /// </summary>
        /// <returns>ブログ記事。</returns>
        public async Task<IList<Article>> FindAll()
        {
            return await this.context.Articles.Include(a => a.Tags).ToListAsync();
        }

        /// <summary>
        /// ブログ内のブログ記事を取得する。
        /// </summary>
        /// <param name="blogId">ブログID。</param>
        /// <returns>ブログ記事。</returns>
        public async Task<IList<Article>> FindByBlogId(int blogId)
        {
            return await this.context.Articles.Include(a => a.Tags).Where(a => a.BlogId == blogId).ToListAsync();
        }

        /// <summary>
        /// ブログ記事IDでブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        public Task<Article> Find(int id)
        {
            return this.context.Articles.Include(a => a.Tags).FirstAsync(a => a.Id == id);
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

        #endregion

        #region 更新系メソッド

        /// <summary>
        /// ブログ記事を登録する。
        /// </summary>
        /// <param name="article">ブログ記事。</param>
        /// <returns>登録したブログ記事。</returns>
        public async Task<Article> Create(Article article)
        {
            article.Id = 0;
            this.context.Articles.Add(article);
            await this.context.SaveChangesAsync();
            return article;
        }

        /// <summary>
        /// ブログ記事を更新する。
        /// </summary>
        /// <param name="article">ブログ記事。</param>
        /// <returns>更新したブログ記事。</returns>
        public async Task<Article> Update(Article article)
        {
            this.context.Entry(article).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
            return article;
        }

        /// <summary>
        /// ブログ記事を削除する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>削除したブログ記事。</returns>
        /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
        public async Task<Article> Delete(int id)
        {
            var article = await this.FindOrFail(id);
            this.context.Articles.Remove(article);
            await this.context.SaveChangesAsync();
            return article;
        }

        #endregion
    }
}
