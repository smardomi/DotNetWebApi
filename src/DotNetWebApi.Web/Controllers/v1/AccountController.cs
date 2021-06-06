#region usings

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.Common.Exceptions;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.IocConfig.Api;
using DotNetWebApi.Services.Contracts;
using DotNetWebApi.Services.Jwt;
using DotNetWebApi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

#endregion

namespace DotNetWebApi.Web.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1")]
    public class AccountController : BaseController
    {
        #region ctor

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userRepository;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly ITokenStoreService _tokenStoreService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userRepository, ITokenFactoryService tokenFactoryService, ITokenStoreService tokenStoreService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _tokenFactoryService = tokenFactoryService;
            _tokenStoreService = tokenStoreService;
        }

        #endregion

        #region Login

        /// <summary>
        /// لاگین شدن و دریافت توکن
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<ActionResult> LoginOAuth2([FromForm] LoginOAuth2 tokenRequest, CancellationToken cancellationToken)
        {
            if (!tokenRequest.Grant_Type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            return await LoginBody(tokenRequest.Username, tokenRequest.Password, cancellationToken);
        }

        /// <summary>
        /// لاگین شدن و دریافت توکن
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> GetToken(Login tokenRequest, CancellationToken cancellationToken)
        {
            return await LoginBody(tokenRequest.Username, tokenRequest.Password, cancellationToken);
        }

        private async Task<ActionResult> LoginBody(string userName, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            if (!user.IsActive)
            {
                throw new BadRequestException("حساب کاربری شماغیر فعال شده است.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

            if (result.Succeeded)
            {
                var token = await GenerateJwtTokensAsync(cancellationToken, user);

                return new JsonResult(token);
            }

            if (result.IsLockedOut)
            {
                throw new BadRequestException("حساب کاربری شما قفل شده است.");
            }

            throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");
        }


        #endregion

        #region RefreshToken

        /// <summary>
        /// دریافت توکن جدید
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> RefreshToken(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            var result = await VerifyAndGenerateToken(tokenRequest, cancellationToken);

            return new JsonResult(result);
        }

        private async Task<JwtTokensData> VerifyAndGenerateToken(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            // validation 4 - validate existence of the token
            var storedToken = await _tokenStoreService.FindTokenAsync(tokenRequest.RefreshToken, cancellationToken);

            if (storedToken == null)
            {
                throw new BadRequestException("Token does not exist");
            }

            // Validation 5 - validate if used
            if (storedToken.IsUsed)
            {
                throw new BadRequestException("Token has been used");
            }

            // Validation 6 - validate if revoked
            if (storedToken.IsRevoked)
            {
                throw new BadRequestException("Token has been revoked");
            }

            // Validation 7 - validate the id
            var tokenHashed = SecurityHelper.GetSha256Hash(tokenRequest.Token);

            if (storedToken.AccessToken != tokenHashed)
            {
                throw new BadRequestException("Token doesn't match");
            }

            // update current token 

            await _tokenStoreService.UpdateUsedToken(storedToken, cancellationToken);

            // Generate a new token
            var dbUser = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            return await GenerateJwtTokensAsync(cancellationToken, dbUser);
        }

        #endregion

        #region ChangePassword

        /// <summary>
        /// تغییر رمز حساب کاربری
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public virtual async Task<ApiResult> ChangePassword(ChangePasswordDto model, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

            await _userManager.UpdateSecurityStampAsync(user);

            return Ok();

        }

        #endregion

        #region Private

        private async Task<JwtTokensData> GenerateJwtTokensAsync(CancellationToken cancellationToken, User user)
        {
            var jwt = await _tokenFactoryService.CreateJwtTokensAsync(user);

            await _tokenStoreService.AddUserTokenAsync(user, jwt, cancellationToken);
            return jwt;
        }

        #endregion
    }
}
