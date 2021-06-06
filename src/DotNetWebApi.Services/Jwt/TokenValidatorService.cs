using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.Common;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace DotNetWebApi.Services.Jwt
{
    public interface ITokenValidatorService
    {
        Task ValidateAsync(TokenValidatedContext context,CancellationToken cancellationToken);
    }

    public class TokenValidatorService : ITokenValidatorService, IScopedDependency
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _usersService;
        private readonly ITokenStoreService _tokenStoreService;

        public TokenValidatorService(IUserService usersService, ITokenStoreService tokenStoreService, SignInManager<User> signInManager)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _tokenStoreService = tokenStoreService;
            _signInManager = signInManager;
            _tokenStoreService.CheckArgumentIsNull(nameof(_tokenStoreService));
        }

        public async Task ValidateAsync(TokenValidatedContext context, CancellationToken cancellationToken)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
            if (!securityStamp.HasValue())
                context.Fail("This token has no security stamp");


            var userIdString = claimsIdentity.GetUserId<int>().ToString();
            if (!int.TryParse(userIdString, out int userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }
            var user = await _usersService.GetByIdAsync(default,userId);
            if (user == null || !user.IsActive)
            {
                // user has changed his/her password/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
            }

            var validatedUser = await _signInManager.ValidateSecurityStampAsync(context.Principal);
            if (validatedUser == null)
                context.Fail("Token security stamp is not valid.");


            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await _tokenStoreService.IsValidTokenAsync(accessToken.RawData, userId, cancellationToken))
            {
                context.Fail("This token is not in our database.");
                return;
            }

            await _usersService.UpdateLastLoginDateAsync(user, cancellationToken);
        }
    }
}