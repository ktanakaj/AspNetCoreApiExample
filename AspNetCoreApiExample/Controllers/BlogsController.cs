﻿// ================================================================================================
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
    using Honememo.AspNetCoreApiExample.Models;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ブログコントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        /// <summary>
        /// ブログリポジトリ。
        /// </summary>
        private readonly BlogRepository blogRepository;

        /// <summary>
        /// リポジトリをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="blogRepository">ブログリポジトリ。</param>
        public BlogsController(BlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }

        // TODO: 更新系APIは、要認証のコントローラを作って移動する

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
        /// <param name="blog">ブログ。</param>
        /// <returns>登録したブログ。</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Blog>> PostBlog(Blog blog)
        {
            await this.blogRepository.Save(blog);
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
            if (id != blog.Id)
            {
                return this.BadRequest();
            }

            await this.blogRepository.Save(blog);
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
            await this.blogRepository.Remove(id);
            return this.NoContent();
        }
    }
}
