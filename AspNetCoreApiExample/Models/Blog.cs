﻿// ================================================================================================
// <summary>
//      ブログモデルクラスソース</summary>
//
// <copyright file="Blog.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Models
{
    /// <summary>
    /// ブログモデルクラス。
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// ブログID。
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// ブログタイトル。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ブログ作者のユーザーID。
        /// </summary>
        public long UserId { get; set; }
    }
}
