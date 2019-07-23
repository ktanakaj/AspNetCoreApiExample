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
    using Microsoft.AspNetCore.Mvc;
    using Honememo.AspNetCoreApiExample.Entities;
    using Honememo.AspNetCoreApiExample.Repositories;

    /// <summary>
    /// ユーザーコントローラクラス。
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region メンバー変数

        /// <summary>
        /// ユーザーリポジトリ。
        /// </summary>
        private readonly UserRepository userRepository;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// リポジトリをDIしてコントローラを生成する。
        /// </summary>
        /// <param name="userRepository">ユーザーリポジトリ。</param>
        public UsersController(UserRepository userRepository)
        {
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
        /// <param name="user">ユーザー。</param>
        /// <returns>登録したユーザー。</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // TODO: 作成と同時にログインするようにする
            await this.userRepository.Create(user);
            return this.CreatedAtAction(nameof(this.GetUser), new { id = user.Id }, user);
        }

        // TODO: 更新APIは、要認証にして自分のみを更新可にする

        /// <summary>
        /// ユーザーを更新する。
        /// </summary>
        /// <param name="id">ユーザーID。</param>
        /// <param name="user">更新するユーザー情報。</param>
        /// <returns>処理結果。</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            user.Id = id;
            await this.userRepository.Update(user);
            return this.NoContent();
        }

        #endregion
    }
}
