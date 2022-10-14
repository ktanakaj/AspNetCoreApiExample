// ================================================================================================
// <summary>
//      ブログ記事検索のリクエストパラメータ用のDTOクラスソース</summary>
//
// <copyright file="ArticleSearchDto.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;

namespace Honememo.AspNetCoreApiExample.Dto
{
    /// <summary>
    /// ブログ記事検索のリクエストパラメータ用のDTOクラス。
    /// </summary>
    public class ArticleSearchDto
    {
        /// <summary>
        /// ブログID。
        /// </summary>
        public int BlogId { get; set; }

        /// <summary>
        /// タグ。
        /// </summary>
        public string? Tag { get; set; }

        /// <summary>
        /// 投稿日時期間開始。
        /// </summary>
        public DateTimeOffset? StartAt { get; set; }

        /// <summary>
        /// 投稿日時期間終了。
        /// </summary>
        public DateTimeOffset? EndAt { get; set; }

        /// <summary>
        /// ページングでスキップする件数。
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Skip { get; set; }

        /// <summary>
        /// ページングで読み込む件数。
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Take { get; set; }
    }
}
