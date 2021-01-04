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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Services;

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
        /// ブログ記事サービス。
        /// </summary>
        private readonly ArticleService articleService;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// サービスを使用するコントローラを生成する。
        /// </summary>
        /// <param name="articleService">ブログ記事サービス。</param>
        public ArticlesController(ArticleService articleService)
        {
            this.articleService = articleService;
        }

        #endregion

        #region APIメソッド

        /// <summary>
        /// ブログ記事一覧を取得する。
        /// </summary>
        /// <param name="query">検索条件。</param>
        /// <returns>ブログ記事一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles([FromQuery] ArticleSearchDto query)
        {
            return (await this.articleService.FindArticles(query)).ToList();
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
            return await this.articleService.FindArticle(id);
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
            var article = await this.articleService.CreateArticle(this.UserId, body);
            return this.CreatedAtAction(nameof(this.GetArticle), new { id = article.Id }, article);
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
            await this.articleService.UpdateArticle(this.UserId, id, body);
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
            await this.articleService.DeleteArticle(this.UserId, id);
            return this.NoContent();
        }

        #endregion
    }
}
