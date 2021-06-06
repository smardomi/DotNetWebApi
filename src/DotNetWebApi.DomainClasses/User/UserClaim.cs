using DotNetWebApi.DomainClasses.Common;
using Microsoft.AspNetCore.Identity;

namespace DotNetWebApi.DomainClasses.User
{
  
    public class UserClaim : IdentityUserClaim<int>, IEntity
    {
        public virtual User User { get; set; }
    }
}