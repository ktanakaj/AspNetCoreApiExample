// ================================================================================================
// <summary>
//      ブログ記事サービスクラスソース</summary>
//
// <copyright file="ArticleService.cs">
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
/// ブログ記事サービスクラス。
/// </summary>
public class ArticleService
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
    public ArticleService(IMapper mapper, AppDbContext context)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// ブログ記事一覧を取得する。
    /// </summary>
    /// <param name="param">検索条件。</param>
    /// <returns>ブログ記事一覧。</returns>
    public async Task<IEnumerable<ArticleDto>> FindArticles(ArticleSearchDto param)
    {
        IQueryable<Article> query = this.context.Articles.Include(a => a.Tags);
        if (param.BlogId > 0)
        {
            query = query.Where(a => a.BlogId == param.BlogId);
        }

        if (!string.IsNullOrWhiteSpace(param.Tag))
        {
            query = query.Where(a => a.Tags.Any(t => t.Name == param.Tag));
        }

        if (param.StartAt != null)
        {
            query = query.Where(a => a.CreatedAt >= param.StartAt);
        }

        if (param.EndAt != null)
        {
            query = query.Where(a => a.CreatedAt <= param.EndAt);
        }

        query = query.OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id);

        if (param.Skip > 0)
        {
            query = query.Skip(param.Skip);
        }

        if (param.Take > 0)
        {
            query = query.Take(param.Take);
        }

        return this.mapper.Map<IEnumerable<ArticleDto>>(await query.ToListAsync());
    }

    /// <summary>
    /// 指定されたブログ記事を取得する。
    /// </summary>
    /// <param name="id">ブログ記事ID。</param>
    /// <returns>ブログ記事。</returns>
    /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
    public async Task<ArticleDto> FindArticle(int id)
    {
        return this.mapper.Map<ArticleDto>(await this.FindOrFail(id));
    }

    /// <summary>
    /// ブログ記事を登録する。
    /// </summary>
    /// <param name="userId">ユーザーID。</param>
    /// <param name="param">ブログ記事情報。</param>
    /// <returns>登録したブログ記事。</returns>
    /// <exception cref="NotFoundException">ブログが存在しない場合。</exception>
    /// <exception cref="ForbiddenException">ユーザーのブログでない場合。</exception>
    public async Task<ArticleDto> CreateArticle(int userId, ArticleNewDto param)
    {
        var blog = await this.context.Blogs.FindAsync(param.BlogId)
            ?? throw new NotFoundException($"BlogId={param.BlogId} is not found");
        if (blog.UserId != userId)
        {
            throw new ForbiddenException($"BlogId={param.BlogId} does not belong to me");
        }

        var article = this.mapper.Map<Article>(param);
        article.Id = 0;
        this.context.Articles.Add(article);
        await this.context.SaveChangesAsync();
        return this.mapper.Map<ArticleDto>(article);
    }

    /// <summary>
    /// ブログ記事を更新する。
    /// </summary>
    /// <param name="userId">ユーザーID。</param>
    /// <param name="articleId">ブログ記事ID。</param>
    /// <param name="param">ブログ記事情報。</param>
    /// <returns>処理結果。</returns>
    /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
    /// <exception cref="ForbiddenException">ユーザーのブログでない場合。</exception>
    public async Task UpdateArticle(int userId, int articleId, ArticleEditDto param)
    {
        var article = await this.FindOrFail(articleId);
        var blog = await this.context.Blogs.FindAsync(article.BlogId)
            ?? throw new NotFoundException($"BlogId={article.BlogId} is not found");
        if (blog.UserId != userId)
        {
            throw new ForbiddenException($"id={articleId} does not belong to me");
        }

        // 追跡済みエンティティを直接変更するため、SaveChangesAsync で差分のみUPDATEされる
        this.mapper.Map(param, article);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// ブログ記事を削除する。
    /// </summary>
    /// <param name="userId">ユーザーID。</param>
    /// <param name="articleId">ブログ記事ID。</param>
    /// <returns>処理結果。</returns>
    /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
    /// <exception cref="ForbiddenException">ユーザーのブログでない場合。</exception>
    public async Task DeleteArticle(int userId, int articleId)
    {
        var article = await this.FindOrFail(articleId);
        var blog = await this.context.Blogs.FindAsync(article.BlogId)
            ?? throw new NotFoundException($"BlogId={article.BlogId} is not found");
        if (blog.UserId != userId)
        {
            throw new ForbiddenException($"id={articleId} does not belong to me");
        }

        this.context.Articles.Remove(article);
        await this.context.SaveChangesAsync();
    }

    /// <summary>
    /// ブログ記事IDでブログ記事を取得する。存在しない場合は例外を投げる。
    /// </summary>
    /// <param name="id">ブログ記事ID。</param>
    /// <returns>ブログ記事。</returns>
    /// <exception cref="NotFoundException">ブログ記事が存在しない場合。</exception>
    private async Task<Article> FindOrFail(int id)
    {
        return await this.context.Articles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new NotFoundException($"id={id} is not found");
    }
}
