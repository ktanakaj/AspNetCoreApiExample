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
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
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
        /// <param name="signInManager">サインインマネージャー。</param>
        /// <param name="userRepository">ユーザーリポジトリ。</param>
        public UsersController(SignInManager<User> signInManager, UserRepository userRepository)
        {
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
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return (await this.userRepository.FindAll()).ToList();
        }

        /// <summary>
        /// 指定されたユーザーを取得する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <returns>ユーザー。</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await this.userRepository.FindOrFail(id);
        }

        /// <summary>
        /// ユーザーを新規登録する。
        /// </summary>
        /// <param name="body">ユーザー登録情報。</param>
        /// <returns>登録したユーザー。</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<User>> CreateUser(CreateUserBody body)
        {
            // ユーザーを登録し、ログイン中の状態にする
            var user = await this.userRepository.CreateBy(body.UserName, body.Password);
            await this.signInManager.SignInAsync(user, false);
            return this.CreatedAtAction(nameof(this.GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// ログインする。
        /// </summary>
        /// <param name="body">認証情報。</param>
        /// <returns>登録したユーザー。</returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<User>> Login(LoginBody body)
        {
            var result = await this.signInManager.PasswordSignInAsync(body.UserName, body.Password, false, false);
            if (!result.Succeeded)
            {
                throw new BadRequestException("name or password is not valid");
            }

            // ※ この時点では this.User は空で使用できない
            return await this.userRepository.FindByName(body.UserName);
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
        public async Task<IActionResult> UpdateUser(UpdateUserBody body)
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
        public async Task<IActionResult> ChangePassword(ChangePasswordBody body)
        {
            await this.userRepository.ChangePassword(this.UserId, body.CurrentPassword, body.NewPassword);
            return this.NoContent();
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// ユーザー登録のリクエストパラメータ。
        /// </summary>
        public class CreateUserBody : LoginBody
        {
        }

        /// <summary>
        /// ユーザー更新のリクエストパラメータ。
        /// </summary>
        public class UpdateUserBody
        {
            /// <summary>
            /// ユーザー名。
            /// </summary>
            [Required]
            [MaxLength(191)]
            public string UserName { get; set; }
        }

        /// <summary>
        /// ログインのリクエストパラメータ。
        /// </summary>
        public class LoginBody
        {
            /// <summary>
            /// ユーザー名。
            /// </summary>
            [Required]
            [MaxLength(191)]
            public string UserName { get; set; }

            /// <summary>
            /// パスワード。
            /// </summary>
            [Required]
            public string Password { get; set; }
        }

        /// <summary>
        /// パスワード変更のリクエストパラメータ。
        /// </summary>
        public class ChangePasswordBody
        {
            /// <summary>
            /// 現在のパスワード。
            /// </summary>
            [Required]
            public string CurrentPassword { get; set; }

            /// <summary>
            /// 新しいパスワード。
            /// </summary>
            [Required]
            public string NewPassword { get; set; }
        }

        #endregion
    }
}
