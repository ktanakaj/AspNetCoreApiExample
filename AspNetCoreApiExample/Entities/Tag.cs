// ================================================================================================
// <summary>
//      タグエンティティクラスソース</summary>
//
// <copyright file="Tag.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honememo.AspNetCoreApiExample.Entities;

/// <summary>
/// タグエンティティクラス。
/// </summary>
/// <remarks>
/// <see cref="Article"/>のサブエンティティ。
/// 記事のタグを表す。主キーは記事IDとタグ名。
/// </remarks>
[Index(nameof(Name), nameof(ArticleId))]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public class Tag
{
    /// <summary>
    /// タグ付けられた記事ID。
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// タグ名。
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// タグ付けられた記事。
    /// </summary>
    public Article Article { get; set; } = null!;

    /// <summary>
    /// エンティティ設定クラス。
    /// </summary>
    public class EntityTypeConfiguration : IEntityTypeConfiguration<Tag>
    {
        /// <summary>
        /// モデル構築時に呼ばれる処理。
        /// </summary>
        /// <param name="builder">エンティティビルダー。</param>
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            // 複合主キーを設定
            builder.HasKey(t => new { t.ArticleId, t.Name });
        }
    }
}
