// ================================================================================================
// <summary>
//      ユーザーサービスクラスソース</summary>
//
// <copyright file="UserService.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

using Honememo.AspNetCoreApiExample.Dto;
using Honememo.AspNetCoreApiExample.Entities;
using Honememo.AspNetCoreApiExample.Exceptions;
using Honememo.AspNetCoreApiExample.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Honememo.AspNetCoreApiExample.Services;

/// <summary>
/// ユーザーサービスクラス。
/// </summary>
public class UserService
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
    /// ユーザーマネージャー。
    /// </summary>
    private readonly UserManager<User> userManager;

    /// <summary>
    /// コンテキスト等を使用するサービスを生成する。
    /// </summary>
    /// <param name="mapper">Mapsterインスタンス。</param>
    /// <param name="context">アプリケーションDBコンテキスト。</param>
    /// <param name="userManager">ユーザーマネージャー。</param>
    public UserService(IMapper mapper, AppDbContext context, UserManager<User> userManager)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    /// <summary>
    /// ユーザー一覧を取得する。
    /// </summary>
    /// <returns>ユーザー一覧。</returns>
    public async Task<IEnumerable<UserDto>> FindUsers()
    {
        return this.mapper.Map<IEnumerable<UserDto>>(
            await this.userManager.Users.OrderBy(u => u.NormalizedUserName).ToListAsync());
    }

    /// <summary>
    /// 指定されたユーザーを取得する。
    /// </summary>
    /// <param name="id">ユーザーID。</param>
    /// <returns>ユーザー。</returns>
    /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
    public async Task<UserDto> FindUser(int id)
    {
        return this.mapper.Map<UserDto>(await this.FindOrFail(id));
    }

    /// <summary>
    /// ユーザーを新規登録する。
    /// </summary>
    /// <param name="param">ユーザー登録情報。</param>
    /// <returns>登録したユーザー。</returns>
    /// <exception cref="BadRequestException">入力値が不正な場合。</exception>
    public async Task<User> CreateUser(UserNewDto param)
    {
        using var transaction = await this.context.Database.BeginTransactionAsync();
        var user = new User()
        {
            UserName = param.UserName,
            LastLogin = DateTimeOffset.UtcNow,
        };
        ThrowBadRequestExceptionIfResultIsNotSucceeded(await this.userManager.CreateAsync(user));
        ThrowBadRequestExceptionIfResultIsNotSucceeded(await this.userManager.AddPasswordAsync(user, param.Password));
        await transaction.CommitAsync();
        return user;
    }

    /// <summary>
    /// ユーザーの情報を変更する。
    /// </summary>
    /// <param name="userId">ユーザーID。</param>
    /// <param name="param">ユーザー変更情報。</param>
    /// <returns>処理結果。</returns>
    /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
    /// <exception cref="BadRequestException">入力値が不正な場合。</exception>
    public async Task UpdateUser(int userId, UserEditDto param)
    {
        var user = await this.FindOrFail(userId);
        ThrowBadRequestExceptionIfResultIsNotSucceeded(
            await this.userManager.SetUserNameAsync(user, param.UserName));
    }

    /// <summary>
    /// ユーザーのパスワードを変更する。
    /// </summary>
    /// <param name="userId">ユーザーID。</param>
    /// <param name="param">パスワード変更情報。</param>
    /// <returns>処理結果。</returns>
    /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
    /// <exception cref="BadRequestException">パスワードが変更条件を満たさない場合。</exception>
    public async Task ChangePassword(int userId, ChangePasswordDto param)
    {
        var user = await this.FindOrFail(userId);
        ThrowBadRequestExceptionIfResultIsNotSucceeded(
            await this.userManager.ChangePasswordAsync(user, param.CurrentPassword, param.NewPassword));
    }

    /// <summary>
    /// ログイン用にユーザーを取得&amp;更新する。
    /// </summary>
    /// <param name="name">ユーザー名。</param>
    /// <returns>ユーザー情報。</returns>
    /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
    public async Task<UserDto> FindAndUpdateForLogin(string name)
    {
        var user = await this.userManager.FindByNameAsync(name)
            ?? throw new NotFoundException($"name={name} is not found");
        user.LastLogin = DateTimeOffset.UtcNow;
        ThrowBadRequestExceptionIfResultIsNotSucceeded(await this.userManager.UpdateAsync(user));
        return this.mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// ユーザーIDでユーザーを取得する。存在しない場合は例外を投げる。
    /// </summary>
    /// <param name="id">ユーザーID。</param>
    /// <returns>ユーザー。</returns>
    /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
    private async Task<User> FindOrFail(int id)
    {
        return await this.userManager.FindByIdAsync(id.ToString())
            ?? throw new NotFoundException($"id={id} is not found");
    }

    /// <summary>
    /// UserManagerの戻り値が失敗の場合に入力値不正例外を投げる。
    /// </summary>
    /// <param name="result">チェックする戻り値。</param>
    private static void ThrowBadRequestExceptionIfResultIsNotSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
