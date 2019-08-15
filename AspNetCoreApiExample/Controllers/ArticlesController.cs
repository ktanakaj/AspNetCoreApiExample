// ================================================================================================
// <summary>
//      ブログ記事コントローラクラスソース</summary>
//
// <copyright file="ArticlesController.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ブログ記事コントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : AppControllerBase
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
        /// リポジトリをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="blogRepository">ブログリポジトリ。</param>
        /// <param name="articleRepository">ブログ記事リポジトリ。</param>
        public ArticlesController(IMapper mapper, BlogRepository blogRepository, ArticleRepository articleRepository)
        {
            this.mapper = mapper;
            this.blogRepository = blogRepository;
            this.articleRepository = articleRepository;
        }

        #endregion

        #region APIメソッド

        /// <summary>
        /// ブログ記事一覧を取得する。
        /// </summary>
        /// <param name="blogId">ブログID。</param>
        /// <returns>ブログ記事一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles([FromQuery] int blogId)
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

            return this.mapper.Map<IEnumerable<ArticleDto>>(results).ToList();
        }

        /// <summary>
        /// 指定されたブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ArticleDto>> GetArticle(int id)
        {
            return this.mapper.Map<ArticleDto>(await this.articleRepository.Find(id));
        }

        /// <summary>
        /// ブログ記事を登録する。
        /// </summary>
        /// <param name="body">ブログ記事情報。</param>
        /// <returns>登録したブログ記事。</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ArticleDto>> PostArticle(ArticleNewDto body)
        {
            var blog = await this.blogRepository.FindOrFail(body.BlogId);
            if (blog.UserId != this.UserId)
            {
                throw new ForbiddenException($"BlogId={body.BlogId} does not belong to me");
            }

            var article = await this.articleRepository.Create(this.mapper.Map<Article>(body));
            return this.CreatedAtAction(nameof(this.GetArticle), new { id = article.Id }, this.mapper.Map<ArticleDto>(article));
        }

        /// <summary>
        /// ブログ記事を更新する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <param name="body">ブログ記事情報。</param>
        /// <returns>処理結果。</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutArticle(int id, ArticleEditDto body)
        {
            var article = await this.articleRepository.FindOrFail(id);
            var blog = await this.blogRepository.FindOrFail(article.BlogId);
            if (blog.UserId != this.UserId)
            {
                throw new ForbiddenException($"id={id} does not belong to me");
            }

            this.mapper.Map(body, article);
            await this.articleRepository.Update(article);
            return this.NoContent();
        }

        /// <summary>
        /// ブログ記事を削除する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>処理結果。</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await this.articleRepository.FindOrFail(id);
            var blog = await this.blogRepository.FindOrFail(article.BlogId);
            if (blog.UserId != this.UserId)
            {
                throw new ForbiddenException($"id={id} does not belong to me");
            }

            await this.articleRepository.Delete(id);
            return this.NoContent();
        }

        #endregion
    }
}
