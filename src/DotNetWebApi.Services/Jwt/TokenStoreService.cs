using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.Common;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DomainClasses.Common;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotNetWebApi.Services.Jwt
{
    public interface ITokenStoreService
    {
        Task AddUserTokenAsync(User user, JwtTokensData jwtToken, CancellationToken cancellationToken);
        Task UpdateUsedToken(RefreshToken refreshToken,CancellationToken cancellationToken);
        Task<bool> IsValidTokenAsync(string accessToken, int userId, CancellationToken cancellationToken);
        Task DeleteExpiredTokensAsync(CancellationToken cancellationToken);
        Task<RefreshToken> FindTokenAsync(string refreshToken,CancellationToken cancellationToken);
        Task DeleteTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource, CancellationToken cancellationToken);
        Task InvalidateUserTokensAsync(int userId, CancellationToken cancellationToken);
        Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue, CancellationToken cancellationToken);
    }

    public class TokenStoreService : ITokenStoreService, IScopedDependency
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly SiteSettings _siteSetting;


        public TokenStoreService(
            ITokenFactoryService tokenFactoryService, IRefreshTokenService refreshTokenService, IOptions<SiteSettings> settings)
        {
            _siteSetting = settings.Value;
            _refreshTokenService = refreshTokenService;
            tokenFactoryService.CheckArgumentIsNull(nameof(tokenFactoryService));
        }

        public async Task AddUserTokenAsync(User user, JwtTokensData jwtToken, CancellationToken cancellationToken)
        {
            if (_siteSetting.JwtSettings.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(user.Id, cancellationToken);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(jwtToken.RefreshToken, cancellationToken);

            await _refreshTokenService.AddRefreshTokenAsync(user, jwtToken, cancellationToken);
        }

        public async Task UpdateUsedToken(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            refreshToken.IsUsed = true;
            await  _refreshTokenService.UpdateAsync(refreshToken, cancellationToken);
        }

        public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            await _refreshTokenService.Entities.Where(x => x.ExpiryDate < now)
                         .ForEachAsync(userToken =>
                         {
                             _refreshTokenService.DeleteAsync(userToken, cancellationToken);
                         }, cancellationToken: cancellationToken);
        }

        public async Task DeleteTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await FindTokenAsync(refreshToken, cancellationToken);
            if (token != null)
            {
                await _refreshTokenService.DeleteAsync(token, cancellationToken);
            }
        }

        public async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshToken, CancellationToken cancellationToken)
        {
            Assert.NotNull(refreshToken,nameof(refreshToken));

            var refreshTokenHash = SecurityHelper.GetSha256Hash(refreshToken);
            await _refreshTokenService.Entities.Where(t => t.Token == refreshTokenHash)
                         .ForEachAsync(userToken =>
                         {
                             _refreshTokenService.DeleteAsync(userToken, cancellationToken);
                         }, cancellationToken: cancellationToken);
        }

        public async Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out int userId))
            {
                if (_siteSetting.JwtSettings.AllowSignOutAllUserActiveClients)
                {
                    await InvalidateUserTokensAsync(userId, cancellationToken);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                var refreshTokenHash = SecurityHelper.GetSha256Hash(refreshTokenValue);
                await DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenHash, cancellationToken);
            }

            await DeleteExpiredTokensAsync(cancellationToken);
        }

        public Task<RefreshToken> FindTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            Assert.NotNull(refreshToken, nameof(RefreshToken));

            var refreshTokenHash = SecurityHelper.GetSha256Hash(refreshToken);

            return _refreshTokenService.Entities.Include(a => a.User).SingleOrDefaultAsync(a => a.Token == refreshTokenHash, cancellationToken: cancellationToken);

        }

        public async Task InvalidateUserTokensAsync(int userId, CancellationToken cancellationToken)
        {
            await _refreshTokenService.Entities.Where(x => x.UserId == userId)
                         .ForEachAsync(userToken =>
                         {
                             _refreshTokenService.DeleteAsync(userToken, cancellationToken);
                         }, cancellationToken: cancellationToken);
        }

        public async Task<bool> IsValidTokenAsync(string accessToken, int userId, CancellationToken cancellationToken)
        {
            var accessTokenHash = SecurityHelper.GetSha256Hash(accessToken);
            var userToken = await _refreshTokenService.Entities.FirstOrDefaultAsync(
                x => x.AccessToken == accessTokenHash && x.UserId == userId, cancellationToken: cancellationToken);
            return userToken?.ExpiryDate >= DateTimeOffset.UtcNow;
        }
    }
}