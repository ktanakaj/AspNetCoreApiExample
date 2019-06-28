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
    using Microsoft.EntityFrameworkCore;
    using Honememo.AspNetCoreApiExample.Models;

    /// <summary>
    /// ブログコントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        /// <summary>
        /// アプリケーションDBコンテキスト。
        /// </summary>
        private readonly AppDbContext context;

        /// <summary>
        /// コンテキストをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="context">アプリケーションDBコンテキスト。</param>
        public BlogsController(AppDbContext context)
        {
            this.context = context;
        }

        // TODO: 更新系APIは、要認証のコントローラを作って移動する

        /// <summary>
        /// ブログ一覧を取得する。
        /// </summary>
        /// <returns>ブログ一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            return await this.context.Blogs.ToListAsync();
        }

        /// <summary>
        /// 指定されたブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Blog>> GetBlog(long id)
        {
            var blog = await this.context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return this.NotFound();
            }

            return blog;
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
            this.context.Blogs.Add(blog);
            await this.context.SaveChangesAsync();
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
        public async Task<IActionResult> PutBlog(long id, Blog blog)
        {
            if (id != blog.Id)
            {
                return this.BadRequest();
            }

            this.context.Entry(blog).State = EntityState.Modified;
            await this.context.SaveChangesAsync();

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
        public async Task<IActionResult> DeleteBlog(long id)
        {
            var blog = await this.context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return this.NotFound();
            }

            this.context.Blogs.Remove(blog);
            await this.context.SaveChangesAsync();

            return this.NoContent();
        }
    }
}
