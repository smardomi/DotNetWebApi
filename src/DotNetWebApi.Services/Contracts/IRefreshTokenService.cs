using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.DataLayer.Repositories;
using DotNetWebApi.DomainClasses.Common;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.Services.Jwt;

namespace DotNetWebApi.Services.Contracts
{
    public interface IRefreshTokenService : IRepository<RefreshToken>
    {
        Task AddRefreshTokenAsync(User user, JwtTokensData jwtToken,CancellationToken cancellationToken);
        Task<RefreshToken> FindTokenAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken());

    }
}