using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.Common;
using DotNetWebApi.Common.Exceptions;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DataLayer.EFContext;
using DotNetWebApi.DataLayer.Repositories;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNetWebApi.Services.Services
{
    public class UserService : Repository<User>, IUserService, IScopedDependency
    {
        protected readonly IUnitOfWork Uow;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;


        public UserService(IUnitOfWork uow, IHttpContextAccessor contextAccessor, UserManager<User> userManager) : base(uow)
        {
            Uow = uow ?? throw new ArgumentNullException(nameof(Uow));
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public override Task AddAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            return _userManager.CreateAsync(entity);
        }

        public Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(password);
            return Table.Where(p => p.UserName == username && p.PasswordHash == passwordHash).SingleOrDefaultAsync(cancellationToken);
        }

        public Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            //user.SecurityStamp = Guid.NewGuid();
            return UpdateAsync(user, cancellationToken);
        }

        public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTime.Now;
            return UpdateAsync(user, cancellationToken);
        }

        public async Task Pro_Test(CancellationToken cancellationToken)
        {
          await  Uow.ExecuteSqlRawAsync("BEGIN");
          
        }

        public int GetCurrentUserId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userIdString = claimsIdentity.GetUserId<int>().ToString();
            return string.IsNullOrWhiteSpace(userIdString) ? 0 : int.Parse(userIdString);
        }

        public ValueTask<User> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            return GetByIdAsync(cancellationToken, userId);
        }

        public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
        {
            var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName, cancellationToken: cancellationToken);
            if (exists)
                throw new BadRequestException("نام کاربری تکراری است");

            var passwordHash = SecurityHelper.GetSha256Hash(password);
            user.PasswordHash = passwordHash;
            await base.AddAsync(user, cancellationToken);
        }
    }
}
