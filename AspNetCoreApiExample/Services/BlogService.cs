// ================================================================================================
// <summary>
//      ブログサービスクラスソース</summary>
//
// <copyright file="BlogService.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ブログサービスクラス。
    /// </summary>
    public class BlogService
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
        /// リポジトリ等をDIしてサービスを生成する。
        /// </summary>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="blogRepository">ブログリポジトリ。</param>
        public BlogService(IMapper mapper, BlogRepository blogRepository)
        {
            this.mapper = mapper;
            this.blogRepository = blogRepository;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ブログ一覧を取得する。
        /// </summary>
        /// <returns>ブログ一覧。</returns>
        public async Task<IEnumerable<BlogDto>> FindBlogs()
        {
            return this.mapper.Map<IEnumerable<BlogDto>>(await this.blogRepository.FindAll());
        }

        /// <summary>
        /// 指定されたブログを取得する。
        /// </summary>
        /// <param name="id">ブログID。</param>
        /// <returns>ブログ。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        public async Task<BlogDto> FindBlog(int id)
        {
            return this.mapper.Map<BlogDto>(await this.blogRepository.FindOrFail(id));
        }

        /// <summary>
        /// ブログを登録する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="param">ブログ情報。</param>
        /// <returns>登録したブログ。</returns>
        public async Task<BlogDto> CreateBlog(int userId, BlogEditDto param)
        {
            var blog = this.mapper.Map<Blog>(param);
            blog.UserId = userId;
            blog = await this.blogRepository.Create(blog);
            return this.mapper.Map<BlogDto>(blog);
        }

        /// <summary>
        /// ブログを更新する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="blogId">ブログID。</param>
        /// <param name="param">ブログ情報。</param>
        /// <returns>処理結果。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        public async Task UpdateBlog(int userId, int blogId, BlogEditDto param)
        {
            var blog = await this.blogRepository.FindOrFail(blogId);
            if (blog.UserId != userId)
            {
                throw new ForbiddenException($"id={blogId} does not belong to me");
            }

            await this.blogRepository.Update(this.mapper.Map(param, blog));
        }

        /// <summary>
        /// ブログを削除する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="blogId">ブログID。</param>
        /// <returns>処理結果。</returns>
        /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
        public async Task DeleteBlog(int userId, int blogId)
        {
            var blog = await this.blogRepository.FindOrFail(blogId);
            if (blog.UserId != userId)
            {
                throw new ForbiddenException($"id={blogId} does not belong to me");
            }

            await this.blogRepository.Delete(blogId);
        }

        #endregion
    }
}
