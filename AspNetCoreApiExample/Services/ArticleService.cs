// ================================================================================================
// <summary>
//      ブログ記事サービスクラスソース</summary>
//
// <copyright file="ArticleService.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ブログ記事サービスクラス。
    /// </summary>
    public class ArticleService
    {
        #region メンバー変数

        /// <summary>
        /// AutoMapperインスタンス。
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// ブログリポジトリ。
        /// </summary>
        private readonly BlogRepository blogRepository;

        /// <summary>
        /// ブログ記事リポジトリ。
        /// </summary>
        private readonly ArticleRepository articleRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// リポジトリ等をDIしてサービスを生成する。
        /// </summary>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="blogRepository">ブログリポジトリ。</param>
        /// <param name="articleRepository">ブログ記事リポジトリ。</param>
        public ArticleService(IMapper mapper, BlogRepository blogRepository, ArticleRepository articleRepository)
        {
            this.mapper = mapper;
            this.blogRepository = blogRepository;
            this.articleRepository = articleRepository;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ブログ記事一覧を取得する。
        /// </summary>
        /// <param name="blogId">ブログID。</param>
        /// <returns>ブログ記事一覧。</returns>
        public async Task<IEnumerable<ArticleDto>> FindArticles(int blogId)
        {
            IList<Article> results;
            if (blogId > 0)
            {
                results = await this.articleRepository.FindByBlogId(blogId);
            }
            else
            {
                results = await this.articleRepository.FindAll();
            }

            return this.mapper.Map<IEnumerable<ArticleDto>>(results);
        }

        /// <summary>
        /// 指定されたブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
        public async Task<ArticleDto> FindArticle(int id)
        {
            return this.mapper.Map<ArticleDto>(await this.articleRepository.Find(id));
        }

        /// <summary>
        /// ブログ記事を登録する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="param">ブログ記事情報。</param>
        /// <returns>登録したブログ記事。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        /// <exception cref="ForbiddenException">ユーザーのブログでない場合。</exception>
        public async Task<ArticleDto> CreateArticle(int userId, ArticleNewDto param)
        {
            var blog = await this.blogRepository.FindOrFail(param.BlogId);
            if (blog.UserId != userId)
            {
                throw new ForbiddenException($"BlogId={param.BlogId} does not belong to me");
            }

            var article = await this.articleRepository.Create(this.mapper.Map<Article>(param));
            return this.mapper.Map<ArticleDto>(article);
        }

        /// <summary>
        /// ブログ記事を更新する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="articleId">ブログ記事ID。</param>
        /// <param name="param">ブログ記事情報。</param>
        /// <returns>処理結果。</returns>
        /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
        /// <exception cref="ForbiddenException">ユーザーのブログでない場合。</exception>
        public async Task UpdateArticle(int userId, int articleId, ArticleEditDto param)
        {
            var article = await this.articleRepository.FindOrFail(articleId);
            var blog = await this.blogRepository.FindOrFail(article.BlogId);
            if (blog.UserId != userId)
            {
                throw new ForbiddenException($"id={articleId} does not belong to me");
            }

            this.mapper.Map(param, article);
            await this.articleRepository.Update(article);
        }

        /// <summary>
        /// ブログ記事を削除する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="articleId">ブログ記事ID。</param>
        /// <returns>処理結果。</returns>
        /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
        /// <exception cref="ForbiddenException">ユーザーのブログでない場合。</exception>
        public async Task DeleteArticle(int userId, int articleId)
        {
            var article = await this.articleRepository.FindOrFail(articleId);
            var blog = await this.blogRepository.FindOrFail(article.BlogId);
            if (blog.UserId != userId)
            {
                throw new ForbiddenException($"id={articleId} does not belong to me");
            }

            await this.articleRepository.Delete(articleId);
        }

        #endregion
    }
}
