// ================================================================================================
// <summary>
//      IHasCreatedAt, IHasUpdatedAt のタイムスタンプを更新するSaveChangesインタセプタークラスソース</summary>
//
// <copyright file="TimeStampInterceptor.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Honememo.AspNetCoreApiExample.Entities;

/// <summary>
/// <see cref="IHasCreatedAt"/>, <see cref="IHasUpdatedAt"/> のタイムスタンプを更新するSaveChangesインタセプタークラス。
/// </summary>
public class TimeStampInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// SaveChanges開始時に呼び出される処理。
    /// </summary>
    /// <param name="eventData">呼び出し元のDbContext情報。</param>
    /// <param name="result">以前のインターセプターの実行結果。</param>
    /// <returns>以前の実行結果をそのまま返す。</returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context == null)
        {
            throw new ArgumentNullException("eventData.Context is null");
        }

        eventData.Context.ChangeTracker.DetectChanges();
        this.TouchChangedEntities(eventData.Context.ChangeTracker);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// SaveChangesAsync開始時に呼び出される処理。
    /// </summary>
    /// <param name="eventData">呼び出し元のDbContext情報。</param>
    /// <param name="result">以前のインターセプターの実行結果。</param>
    /// <param name="cancellationToken">処理中断用のトークン。</param>
    /// <returns>以前の実行結果をそのまま返す。</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context == null)
        {
            throw new ArgumentNullException("eventData.Context is null");
        }

        eventData.Context.ChangeTracker.DetectChanges();
        this.TouchChangedEntities(eventData.Context.ChangeTracker);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 変更されているエンティティの登録日時/更新日時を更新する。
    /// </summary>
    private void TouchChangedEntities(ChangeTracker changeTracker)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entity in changeTracker.Entries()
            .Where(x => x.Entity is IHasCreatedAt e && x.State == EntityState.Added && e.CreatedAt == default))
        {
            ((IHasCreatedAt)entity.Entity).CreatedAt = now;
        }

        foreach (var entity in changeTracker.Entries()
            .Where(x => x.Entity is IHasUpdatedAt && (x.State == EntityState.Added || x.State == EntityState.Modified)))
        {
            ((IHasUpdatedAt)entity.Entity).UpdatedAt = now;
        }
    }
}