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
    using System.Linq;
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
            this.CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)));
            this.CreateMap<UserNewDto, User>();
            this.CreateMap<UserEditDto, User>();
            this.CreateMap<BlogEditDto, Blog>();
            this.CreateMap<ArticleNewDto, Article>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Distinct().Select(t => new Tag() { Name = t })));
            this.CreateMap<ArticleEditDto, Article>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Distinct().Select(t => new Tag() { Name = t })));
        }
    }
}
