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
        /// 指定されたユーザーを取得する。
        /// </summary>
        /// <param name="name">ユーザー名。</param>
        /// <returns>ユーザー。</returns>
        public async Task<UserDto> FindUser(string name)
        {
            return this.mapper.Map<UserDto>(await this.userRepository.FindByName(name));
        }

        /// <summary>
        /// ユーザーを新規登録する。
        /// </summary>
        /// <param name="param">ユーザー登録情報。</param>
        /// <returns>登録したユーザー。</returns>
        public async Task<User> CreateUser(UserNewDto param)
        {
            return await this.userRepository.CreateBy(param.UserName, param.Password);
        }

        /// <summary>
        /// ユーザーの情報を変更する。
        /// </summary>
        /// <param name="userId">ユーザーID。</param>
        /// <param name="param">ユーザー変更情報。</param>
        /// <returns>処理結果。</returns>
        /// <exception cref="NotFoundException">ユーザーが存在しない場合。</exception>
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
        public async Task ChangePassword(int userId, ChangePasswordDto param)
        {
            await this.userRepository.ChangePassword(userId, param.CurrentPassword, param.NewPassword);
        }

        #endregion
    }
}
