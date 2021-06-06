using System.Linq;
using DotNetWebApi.Common;
using DotNetWebApi.DomainClasses.User;
using DotNetWebApi.Services.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotNetWebApi.Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IOptionsSnapshot<SiteSettings> _userSeedOptions;


        public UserDataInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, IOptionsSnapshot<SiteSettings> userSeedOptions)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _userSeedOptions = userSeedOptions;
        }

        public void InitializeData()
        {
            var adminUserSeed = _userSeedOptions.Value.AdminUserSeed;
            var clientUserSeed = _userSeedOptions.Value.ClientUserSeed;

            if (!_roleManager.RoleExistsAsync(nameof(CustomRoles.Admin)).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new Role { Name = nameof(CustomRoles.Admin), Description = nameof(CustomRoles.AdminDescription) }).GetAwaiter().GetResult();
            }

            if (!_roleManager.RoleExistsAsync(nameof(CustomRoles.User)).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new Role { Name = nameof(CustomRoles.User), Description = nameof(CustomRoles.UserDescription) }).GetAwaiter().GetResult();
            }

            if (!_userManager.Users.AsNoTracking().Any(p => p.UserName == nameof(CustomRoles.Admin))) 
            {
                var user = new User
                {
                    FullName = adminUserSeed.FullName,
                    UserName = adminUserSeed.Username,
                    Email = adminUserSeed.Email
                };
                _userManager.CreateAsync(user, adminUserSeed.Password).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, nameof(CustomRoles.Admin)).GetAwaiter().GetResult();
            }

            if (_userManager.Users.AsNoTracking().Any(p => p.UserName == nameof(CustomRoles.User))) return;
            {
                var user = new User
                {
                    FullName = clientUserSeed.FullName,
                    UserName = clientUserSeed.Username,
                    Email = clientUserSeed.Email
                };
                _userManager.CreateAsync(user, clientUserSeed.Password).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, nameof(CustomRoles.User)).GetAwaiter().GetResult();
            }
        }
    }
}