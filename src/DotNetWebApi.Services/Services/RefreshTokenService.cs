using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.Common;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DataLayer.EFContext;
using DotNetWebApi.DataLayer.Repositories;
using DotNetWebApi.DomainClasses.Common;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.Services.Contracts;
using DotNetWebApi.Services.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotNetWebApi.Services.Services
{
    public class RefreshTokenService : Repository<RefreshToken>, IRefreshTokenService, IScopedDependency
    {
        protected readonly IUnitOfWork Uow;
        private readonly SiteSettings _siteSetting;


        public RefreshTokenService(IUnitOfWork uow,IOptions<SiteSettings> settings) : base(uow)
        {
            Uow = uow ?? throw new ArgumentNullException(nameof(Uow));
            _siteSetting = settings.Value; 
        }

        public async Task AddRefreshTokenAsync(User user, JwtTokensData jwtToken, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken()
            {
                AccessToken = SecurityHelper.GetSha256Hash(jwtToken.AccessToken),
                IsUsed = false,
                UserId = user.Id,
                CreateDate = DateTime.UtcNow,
                ExpiryDate = DateTime.Now.AddDays(_siteSetting.JwtSettings.RefreshTokenExpirationDays),
                IsRevoked = false,
                Token = SecurityHelper.GetSha256Hash(jwtToken.RefreshToken)
            };

            await AddAsync(refreshToken, cancellationToken);
        }

        public Task<RefreshToken> FindTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            Assert.NotNull(refreshToken, nameof(RefreshToken));

            var refreshTokenHash = SecurityHelper.GetSha256Hash(refreshToken);

            return Entities.Include(a=>a.User).SingleOrDefaultAsync(a=>a.Token == refreshTokenHash, cancellationToken: cancellationToken);
        }

    }
}
