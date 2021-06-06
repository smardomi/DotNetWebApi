using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotNetWebApi.DomainClasses.Common;
using Microsoft.AspNetCore.Identity;

namespace DotNetWebApi.DomainClasses.User
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        public User()
        {
            IsActive = true;
        }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<UserClaim> Claims { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }

}
