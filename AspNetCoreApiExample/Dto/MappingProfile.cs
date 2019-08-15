// ================================================================================================
// <summary>
//      AutoMapperマッピングプロファイルクラスソース</summary>
//
// <copyright file="MappingProfile.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Dto
{
    using AutoMapper;
    using Honememo.AspNetCoreApiExample.Entities;

    /// <summary>
    /// AutoMapperマッピングプロファイルクラス。
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// プロファイルを生成する。
        /// </summary>
        public MappingProfile()
        {
            this.CreateMap<User, UserDto>();
            this.CreateMap<Blog, BlogDto>();
            this.CreateMap<Article, ArticleDto>();
            this.CreateMap<UserNewDto, User>();
            this.CreateMap<UserEditDto, User>();
            this.CreateMap<BlogEditDto, Blog>();
            this.CreateMap<ArticleNewDto, Article>();
            this.CreateMap<ArticleEditDto, Article>();
        }
    }
}
