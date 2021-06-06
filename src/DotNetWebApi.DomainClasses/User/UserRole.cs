using DotNetWebApi.DomainClasses.Common;
using Microsoft.AspNetCore.Identity;

namespace DotNetWebApi.DomainClasses.User
{
 
    public class UserRole : IdentityUserRole<int>, IEntity
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
    public class UserLogin : IdentityUserLogin<int>
    {
    }
    public class RoleClaim : IdentityRoleClaim<int>
    {
    }
    public class UserToken : IdentityUserToken<int>
    {
    }
}