// ================================================================================================
// <summary>
//      ブログコントローラクラスソース</summary>
//
// <copyright file="BlogsController.cs">
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
    /// ブログコントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : AppControllerBase
    {
        #region メンバー変数

        /// <summary>
        /// ブログリポジトリ。
        /// </summary>
        private readonly BlogService blogService;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// サービスを使用するコントローラを生成する。
        /// </summary>
        /// <param name="blogService">ブログサービス。</param>
        public BlogsController(BlogService blogService)
        {
            this.blogService = blogService;
        }

        #endregion

        #region APIメソッド

        /// <summary>
        /// ブログ一覧を取得する。
        /// </summary>
        /// <returns>ブログ一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogs()
        {
            return (await this.blogService.FindBlogs()).ToList();
        }

        /// <summary>
        /// 指定されたブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BlogDto>> GetBlog(int id)
        {
            return await this.blogService.FindBlog(id);
        }

        /// <summary>
        /// ブログを登録する。
        /// </summary>
        /// <param name="body">ブログ情報。</param>
        /// <returns>登録したブログ。</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BlogDto>> PostBlog(BlogEditDto body)
        {
            var blog = await this.blogService.CreateBlog(this.UserId, body);
            return this.CreatedAtAction(nameof(this.GetBlog), new { id = blog.Id }, blog);
        }

        /// <summary>
        /// ブログを更新する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <param name="body">ブログ情報。</param>
        /// <returns>処理結果。</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutBlog(int id, BlogEditDto body)
        {
            await this.blogService.UpdateBlog(this.UserId, id, body);
            return this.NoContent();
        }

        /// <summary>
        /// ブログを削除する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>処理結果。</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await this.blogService.DeleteBlog(this.UserId, id);
            return this.NoContent();
        }

        #endregion
    }
}
