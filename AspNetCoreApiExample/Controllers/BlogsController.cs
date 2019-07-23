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
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ブログコントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        #region メンバー変数

        /// <summary>
        /// ブログリポジトリ。
        /// </summary>
        private readonly BlogRepository blogRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// リポジトリをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="blogRepository">ブログリポジトリ。</param>
        public BlogsController(BlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }

        #endregion

        #region APIメソッド

        /// <summary>
        /// ブログ一覧を取得する。
        /// </summary>
        /// <returns>ブログ一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            return (await this.blogRepository.FindAll()).ToList();
        }

        /// <summary>
        /// 指定されたブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            return await this.blogRepository.FindOrFail(id);
        }

        // TODO: 更新系APIには認証かける

        /// <summary>
        /// ブログを登録する。
        /// </summary>
        /// <param name="blog">ブログ。</param>
        /// <returns>登録したブログ。</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Blog>> PostBlog(Blog blog)
        {
            await this.blogRepository.Create(blog);
            return this.CreatedAtAction(nameof(this.GetBlog), new { id = blog.Id }, blog);
        }

        /// <summary>
        /// ブログを更新する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <param name="blog">更新するブログ情報。</param>
        /// <returns>処理結果。</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutBlog(int id, Blog blog)
        {
            blog.Id = id;
            await this.blogRepository.Update(blog);
            return this.NoContent();
        }

        /// <summary>
        /// ブログを削除する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>処理結果。</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await this.blogRepository.Delete(id);
            return this.NoContent();
        }

        #endregion
    }
}
