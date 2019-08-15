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
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Dto;
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
        /// AutoMapperインスタンス。
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// ブログリポジトリ。
        /// </summary>
        private readonly BlogRepository blogRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// リポジトリをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="blogRepository">ブログリポジトリ。</param>
        public BlogsController(IMapper mapper, BlogRepository blogRepository)
        {
            this.mapper = mapper;
            this.blogRepository = blogRepository;
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
            return this.mapper.Map<IEnumerable<BlogDto>>(await this.blogRepository.FindAll()).ToList();
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
            return this.mapper.Map<BlogDto>(await this.blogRepository.FindOrFail(id));
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
            var blog = this.mapper.Map<Blog>(body);
            blog.UserId = this.UserId;
            blog = await this.blogRepository.Create(blog);
            return this.CreatedAtAction(nameof(this.GetBlog), new { id = blog.Id }, this.mapper.Map<BlogDto>(blog));
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
            var blog = await this.blogRepository.FindOrFail(id);
            if (blog.UserId != this.UserId)
            {
                throw new ForbiddenException($"id={id} does not belong to me");
            }

            await this.blogRepository.Update(this.mapper.Map(body, blog));
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
    }
}
