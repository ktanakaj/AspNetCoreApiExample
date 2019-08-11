// ================================================================================================
// <summary>
//      ユーザーエンティティクラスソース</summary>
//
// <copyright file="User.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Entities
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;

    /// <summary>
    /// ユーザーエンティティクラス。
    /// </summary>
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// ユーザーのブログ。
        /// </summary>
        [JsonIgnore]
        public ICollection<Blog> Blogs { get; set; }
    }
}
