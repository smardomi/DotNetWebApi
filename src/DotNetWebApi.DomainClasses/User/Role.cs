using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotNetWebApi.DomainClasses.Common;
using Microsoft.AspNetCore.Identity;

namespace DotNetWebApi.DomainClasses.User
{
    public class Role : IdentityRole<int>, IEntity
    {
        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public virtual ICollection<UserRole> Users { get; set; }

    }

}
