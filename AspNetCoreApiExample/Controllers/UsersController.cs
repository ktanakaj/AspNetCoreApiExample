// ================================================================================================
// <summary>
//      ユーザーコントローラクラスソース</summary>
//
// <copyright file="UsersController.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Honememo.AspNetCoreApiExample.Dto;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Repositories;
    using Honememo.AspNetCoreApiExample.Exceptions;

    /// <summary>
    /// ユーザーコントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : AppControllerBase
    {
        #region メンバー変数

        /// <summary>
        /// AutoMapperインスタンス。
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// サインインマネージャー。
        /// </summary>
        private readonly SignInManager<User> signInManager;

        /// <summary>
        /// ユーザーリポジトリ。
        /// </summary>
        private readonly UserRepository userRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// リポジトリ等をDIしてコントローラを生成する。
        /// </summary>
        /// <param name="mapper">AutoMapperインスタンス。</param>
        /// <param name="signInManager">サインインマネージャー。</param>
        /// <param name="userRepository">ユーザーリポジトリ。</param>
        public UsersController(IMapper mapper, SignInManager<User> signInManager, UserRepository userRepository)
        {
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.userRepository = userRepository;
        }

        #endregion

        #region APIメソッド

        /// <summary>
        /// ユーザー一覧を取得する。
        /// </summary>
        /// <returns>ユーザー一覧。</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return this.mapper.Map<IEnumerable<UserDto>>(await this.userRepository.FindAll()).ToList();
        }

        /// <summary>
        /// 指定されたユーザーを取得する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>ユーザー。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            return this.mapper.Map<UserDto>(await this.userRepository.FindOrFail(id));
        }

        /// <summary>
        /// ユーザーを新規登録する。
        /// </summary>
        /// <param name="body">ユーザー登録情報。</param>
        /// <returns>登録したユーザー。</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> CreateUser(UserNewDto body)
        {
            // ユーザーを登録し、ログイン中の状態にする
            var user = await this.userRepository.CreateBy(body.UserName, body.Password);
            await this.signInManager.SignInAsync(user, false);
            return this.CreatedAtAction(nameof(this.GetUser), new { id = user.Id }, this.mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// ログインする。
        /// </summary>
        /// <param name="body">認証情報。</param>
        /// <returns>登録したユーザー。</returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> Login(LoginDto body)
        {
            var result = await this.signInManager.PasswordSignInAsync(body.UserName, body.Password, false, false);
            if (!result.Succeeded)
            {
                throw new BadRequestException("name or password is not valid");
            }

            // ※ この時点では this.User は空で使用できない
            return this.mapper.Map<UserDto>(await this.userRepository.FindByName(body.UserName));
        }

        /// <summary>
        /// ログアウトする。
        /// </summary>
        /// <returns>処理状態。</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.Ok();
        }

        /// <summary>
        /// 認証中ユーザーの情報を変更する。
        /// </summary>
        /// <param name="body">ユーザー変更情報。</param>
        /// <returns>処理結果。</returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUser(UserEditDto body)
        {
            // ※ 現状ユーザー名の変更のみ対応
            await this.userRepository.ChangeUserName(this.UserId, body.UserName);
            return this.NoContent();
        }

        /// <summary>
        /// 認証中ユーザーのパスワードを変更する。
        /// </summary>
        /// <param name="body">パスワード変更情報。</param>
        /// <returns>処理結果。</returns>
        [HttpPut("password")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto body)
        {
            await this.userRepository.ChangePassword(this.UserId, body.CurrentPassword, body.NewPassword);
            return this.NoContent();
        }

        #endregion
    }
}
