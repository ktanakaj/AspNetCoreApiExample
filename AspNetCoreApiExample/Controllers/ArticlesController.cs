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
    using Microsoft.EntityFrameworkCore;
    using Honememo.AspNetCoreApiExample.Models;

    /// <summary>
    /// ブログ記事コントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        /// <summary>
        /// ブログ記事コンテキスト。
        /// </summary>
        private readonly ArticleContext context;

        /// <summary>
        /// コンテキストをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="context">ブログ記事コンテキスト。</param>
        public ArticlesController(ArticleContext context)
        {
            this.context = context;
        }

        // TODO: 更新系APIは、要認証のコントローラを作って移動する

        /// <summary>
        /// ブログ記事一覧を取得する。
        /// </summary>
        /// <param name="blogId">ブログID。</param>
        /// <returns>ブログ記事一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles(long blogId)
        {
            return await this.context.Articles.ToListAsync();
        }

        /// <summary>
        /// 指定されたブログ記事を取得する。
        /// </summary>
        /// <param name="id">ブログ記事ID。</param>
        /// <returns>ブログ記事。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Article>> GetArticle(long id)
        {
            var article = await this.context.Articles.FindAsync(id);

            if (article == null)
            {
                return this.NotFound();
            }

            return article;
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
            this.context.Articles.Add(article);
            await this.context.SaveChangesAsync();
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

            this.context.Entry(article).State = EntityState.Modified;
            await this.context.SaveChangesAsync();

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
        public async Task<IActionResult> DeleteArticle(long id)
        {
            var article = await this.context.Articles.FindAsync(id);

            if (article == null)
            {
                return this.NotFound();
            }

            this.context.Articles.Remove(article);
            await this.context.SaveChangesAsync();

            return this.NoContent();
        }
    }
}
