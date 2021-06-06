﻿using System;

namespace DotNetWebApi.DomainClasses.Common
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; } // Linked to the AspNet Identity User Id
        public string Token { get; set; }
        public string AccessToken { get; set; } // Map the token with jwtId
        public bool IsUsed { get; set; } // if its used we dont want generate a new Jwt token with the same refresh token
        public bool IsRevoked { get; set; } // if it has been revoke for security reasons
        public DateTime CreateDate { get; set; }
        public DateTime ExpiryDate { get; set; } // Refresh token is long lived it could last for months.

        public User.User User { get; set; }
    }

}
