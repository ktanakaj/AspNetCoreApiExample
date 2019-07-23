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
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Models;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ブログ記事コントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        /// <summary>
        /// ブログ記事リポジトリ。
        /// </summary>
        private readonly ArticleRepository articleRepository;

        /// <summary>
        /// リポジトリをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="articleRepository">ブログ記事リポジトリ。</param>
        public ArticlesController(ArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        // TODO: 更新系APIは、要認証のコントローラを作って移動する

        /// <summary>
        /// ブログ記事一覧を取得する。
        /// </summary>
        /// <param name="blogId">ブログID。</param>
        /// <returns>ブログ記事一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles(int blogId)
        {
            return (await this.articleRepository.FindByBlogId(blogId)).ToList();
        }

        /// <summary>
        /// 指定されたブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            return await this.articleRepository.Find(id);
        }

        /// <summary>
        /// ブログ記事を登録する。
        /// </summary>
        /// <param name="article">ブログ記事。</param>
        /// <returns>登録したブログ記事。</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            await this.articleRepository.Save(article);
            return this.CreatedAtAction(nameof(this.GetArticle), new { id = article.Id }, article);
        }

        /// <summary>
        /// ブログ記事を更新する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <param name="article">更新するブログ記事。</param>
        /// <returns>処理結果。</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutArticle(long id, Article article)
        {
            if (id != article.Id)
            {
                return this.BadRequest();
            }

            await this.articleRepository.Save(article);
            return this.NoContent();
        }

        /// <summary>
        /// ブログ記事を削除する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>処理結果。</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await this.articleRepository.Remove(id);
            return this.NoContent();
        }
    }
}
