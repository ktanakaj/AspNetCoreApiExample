// ================================================================================================
// <summary>
//      ブログサービスクラスソース</summary>
//
// <copyright file="BlogService.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Honememo.AspNetCoreApiExample.Dto;
using Honememo.AspNetCoreApiExample.Entities;
using Honememo.AspNetCoreApiExample.Exceptions;
using Honememo.AspNetCoreApiExample.Repositories;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Services;

/// <summary>
/// ブログサービスクラス。
/// </summary>
public class BlogService
{
    /// <summary>
    /// Mapsterインスタンス。
    /// </summary>
    private readonly IMapper mapper;

    /// <summary>
    /// アプリケーションDBコンテキスト。
    /// </summary>
    private readonly AppDbContext context;

    /// <summary>
    /// コンテキスト等を使用するサービスを生成する。
    /// </summary>
    /// <param name="mapper">Mapsterインスタンス。</param>
    /// <param name="context">アプリケーションDBコンテキスト。</param>
    public BlogService(IMapper mapper, AppDbContext context)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// ブログ一覧を取得する。
    /// </summary>
    /// <returns>ブログ一覧。</returns>
    public async Task<IEnumerable<BlogDto>> FindBlogs()
    {
        return this.mapper.Map<IEnumerable<BlogDto>>(
            await this.context.Blogs.OrderBy(b => b.Name).ThenBy(b => b.UserId).ToListAsync());
    }

    /// <summary>
    /// 指定されたブログを取得する。
    /// </summary>
    /// <param name="id">ブログID。</param>
    /// <returns>ブログ。</returns>
    /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
    public async Task<BlogDto> FindBlog(int id)
    {
        return this.mapper.Map<BlogDto>(await this.FindOrFail(id));
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
        blog.Id = 0;
        blog.UserId = userId;
        this.context.Blogs.Add(blog);
        await this.context.SaveChangesAsync();
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
        var blog = await this.FindOrFail(blogId);
        if (blog.UserId != userId)
        {
            throw new ForbiddenException($"id={blogId} does not belong to me");
        }

        // 追跡済みエンティティを直接変更するため、SaveChangesAsync で差分のみUPDATEされる
        this.mapper.Map(param, blog);
        await this.context.SaveChangesAsync();
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
        var blog = await this.FindOrFail(blogId);
        if (blog.UserId != userId)
        {
            throw new ForbiddenException($"id={blogId} does not belong to me");
        }

        this.context.Blogs.Remove(blog);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// ブログIDでブログを取得する。存在しない場合は例外を投げる。
    /// </summary>
    /// <param name="id">ブログID。</param>
    /// <returns>ブログ。</returns>
    /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
    private async Task<Blog> FindOrFail(int id)
    {
        return await this.context.Blogs.FindAsync(id)
            ?? throw new NotFoundException($"id={id} is not found");
    }
}
