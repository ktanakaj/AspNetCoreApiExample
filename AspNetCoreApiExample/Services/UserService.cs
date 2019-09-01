// ================================================================================================
// <summary>
//      ユーザーサービスクラスソース</summary>
//
// <copyright file="UserService.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Exceptions;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ユーザーサービスクラス。
    /// </summary>
    public class UserService
    {
        #region メンバー変数

        /// <summary>
        /// AutoMapperインスタンス。
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// ユーザーリポジトリ。
        /// </summary>
        private readonly UserRepository userRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// リポジトリ等をDIしてサービスを生成する。
        /// </summary>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="userRepository">ユーザーリポジトリ。</param>
        public UserService(IMapper mapper, UserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ユーザー一覧を取得する。
        /// </summary>
        /// <returns>ユーザー一覧。</returns>
        public async Task<IEnumerable<UserDto>> FindUsers()
        {
            return this.mapper.Map<IEnumerable<UserDto>>(await this.userRepository.FindAll());
        }

        /// <summary>
        /// 指定されたユーザーを取得する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>ユーザー。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
        public async Task<UserDto> FindUser(int id)
        {
            return this.mapper.Map<UserDto>(await this.userRepository.FindOrFail(id));
        }

        /// <summary>
        /// ユーザーを新規登録する。
        /// </summary>
        /// <param name="param">ユーザー登録情報。</param>
        /// <returns>登録したユーザー。</returns>
        /// <exception cref="BadRequestException">入力値が不正な場合。</exception>
        public async Task<User> CreateUser(UserNewDto param)
        {
            // FIXME: CreateBy()は本当はトランザクションが必要。
            //        しかしIUnitOfWorkの仕組みでは機能せず？
            //        TransactionScopeなら行けるようだが、ライブラリが未対応なので対応待ち。
            return await this.userRepository.CreateBy(param.UserName, param.Password);
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
            // ※ 現状ユーザー名の変更のみ対応
            await this.userRepository.ChangeUserName(userId, param.UserName);
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
            await this.userRepository.ChangePassword(userId, param.CurrentPassword, param.NewPassword);
        }

        /// <summary>
        /// ログイン用にユーザーを取得&amp;更新する。
        /// </summary>
        /// <param name="name">ユーザー名。</param>
        /// <returns>ユーザー情報。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
        public async Task<UserDto> FindAndUpdateForLogin(string name)
        {
            var user = await this.userRepository.FindByName(name);
            if (user == null)
            {
                throw new NotFoundException($"name = {name} is not found");
            }

            user.LastLogin = DateTimeOffset.UtcNow;
            return this.mapper.Map<UserDto>(await this.userRepository.Update(user));
        }

        #endregion
    }
}
