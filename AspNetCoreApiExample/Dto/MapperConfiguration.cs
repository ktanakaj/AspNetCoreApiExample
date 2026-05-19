// ================================================================================================
// <summary>
//      Mapsterマッピング設定クラスソース</summary>
//
// <copyright file="MapperConfiguration.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Honememo.AspNetCoreApiExample.Entities;
using Mapster;

namespace Honememo.AspNetCoreApiExample.Dto;

/// <summary>
/// Mapsterマッピング設定クラス。
/// </summary>
public class MapperConfiguration : IRegister
{
    /// <summary>
    /// マッピング設定を登録する。
    /// </summary>
    /// <param name="config">マッピング設定。</param>
    public virtual void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        config.NewConfig<Blog, BlogDto>();
        config.NewConfig<Article, ArticleDto>()
            .Map(dest => dest.Tags, src => src.Tags.Select(t => t.Name));
        config.NewConfig<UserNewDto, User>();
        config.NewConfig<UserEditDto, User>();
        config.NewConfig<BlogEditDto, Blog>();
        config.NewConfig<ArticleNewDto, Article>()
            .Map(dest => dest.Tags, src => src.Tags.Distinct().Select(t => new Tag() { Name = t }));
        config.NewConfig<ArticleEditDto, Article>()
            .Map(dest => dest.Tags, src => src.Tags.Distinct().Select(t => new Tag() { Name = t }));
    }
}
