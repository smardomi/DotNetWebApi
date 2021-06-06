using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.DataLayer.Repositories;
using DotNetWebApi.DomainClasses.User;

namespace DotNetWebApi.Services.Contracts
{
    public interface IUserService : IRepository<User>
    {
        Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken);
        Task AddAsync(User user, string password, CancellationToken cancellationToken);
        Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken);
        Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);
        ValueTask<User> GetCurrentUserAsync(CancellationToken cancellationToken = default);
        int GetCurrentUserId();
        Task Pro_Test(CancellationToken cancellationToken);
    }
}