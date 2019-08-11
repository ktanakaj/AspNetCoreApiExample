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
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;
    using Honememo.AspNetCoreApiExample.Repositories;

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

        /// <summary>
        /// ブログを登録する。
        /// </summary>
        /// <param name="body">ブログ情報。</param>
        /// <returns>登録したブログ。</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Blog>> PostBlog(BlogBody body)
        {
            var blog = await this.blogRepository.Create(new Blog()
            {
                Name = body.Name,
                UserId = this.UserId,
            });
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
        public async Task<IActionResult> PutBlog(int id, BlogBody body)
        {
            var blog = await this.blogRepository.FindOrFail(id);
            if (blog.UserId != this.UserId)
            {
                throw new ForbiddenException($"id={id} does not belong to me");
            }

            blog.Name = body.Name;
            await this.blogRepository.Update(blog);
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
            var blog = await this.blogRepository.FindOrFail(id);
            if (blog.UserId != this.UserId)
            {
                throw new ForbiddenException($"id={id} does not belong to me");
            }

            await this.blogRepository.Delete(id);
            return this.NoContent();
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// ブログ登録/編集のリクエストパラメータ。
        /// </summary>
        public class BlogBody
        {
            /// <summary>
            /// ブログタイトル。
            /// </summary>
            [Required]
            [MaxLength(191)]
            public string Name { get; set; }
        }

        #endregion
    }
}
