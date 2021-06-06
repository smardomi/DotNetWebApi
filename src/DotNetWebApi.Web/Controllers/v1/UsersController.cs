#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DotNetWebApi.Common.Exceptions;
using DotNetWebApi.Common.Pagination;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DataLayer.Repositories;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.IocConfig.Api;
using DotNetWebApi.Services.Constants;
using DotNetWebApi.Services.Contracts;
using DotNetWebApi.Services.Jwt;
using DotNetWebApi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#endregion

namespace DotNetWebApi.Web.Controllers.v1
{
    /// <summary>
    /// ایجاد، ویرایش و حذف کاربران ( مختص مدیر سیستم )
    /// </summary>
    [ApiVersion("1")]
    [Authorize(Roles = nameof(CustomRoles.Admin))]
    public class UsersController : BaseController
    {
        #region ctor

        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userManager"></param>
        public UsersController(IUserService userService, UserManager<User> userManager)
        {
            this._userService = userService;
            this._userManager = userManager;
        }

        #endregion

        #region Get

        /// <summary>
        /// دریافت لیست کاربران
        /// </summary>
        /// <remarks>این سرویس لیست کاربران موجود در سیستم را به صورت صفحه بندی شده بر میگرداند.</remarks>
        /// <param name="urlQueryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetUsersList")]
        public virtual async Task<ActionResult<List<User>>> Get([FromQuery] UrlQueryPagingParameters urlQueryParameters, CancellationToken cancellationToken)
        {
            var pagedModel = await _userService.TableNoTracking
                .OrderByDescending(a => a.Id)
                .PaginateAsync(urlQueryParameters.Limit, urlQueryParameters.Page, cancellationToken);

            var pagedModelWithLinks = GeneratePageLinks(urlQueryParameters, pagedModel, "GetUsersList");

            return Ok(pagedModelWithLinks);
        }

        #endregion

        #region Get

        /// <summary>
        /// دریافت مشخصات یه کاربر خاص
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(cancellationToken, id);
            if (user == null)
                return NotFound();

            await _userManager.UpdateSecurityStampAsync(user);

            return user;
        }

        #endregion

        #region Create

        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FullName = userDto.FullName,
                UserName = userDto.UserName,
                Email = userDto.Email
            };
            var result = await _userManager.CreateAsync(user, userDto.Password);

            var result3 = await _userManager.AddToRoleAsync(user, nameof(CustomRoles.User));

            return user;
        }

        #endregion

        #region Update

        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        public virtual async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var updateUser = await _userService.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = user.UserName;
            updateUser.PasswordHash = user.PasswordHash;
            updateUser.FullName = user.FullName;
            updateUser.IsActive = user.IsActive;
            updateUser.LastLoginDate = user.LastLoginDate;

            await _userService.UpdateAsync(updateUser, cancellationToken);

            return Ok();
        }

        #endregion

        #region Delete

        /// <summary>
        /// حذف کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(cancellationToken, id);
            await _userService.DeleteAsync(user, cancellationToken);

            return Ok();
        }

        #endregion

    }
}
