using System;
using System.IdentityModel.Tokens.Jwt;
using DotNetWebApi.Common.Utilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DotNetWebApi.Services.Jwt
{
    public class JwtTokensData
    {

        public JwtTokensData(SecurityToken securityToken)
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
            RefreshToken = SecurityHelper.CreateRandomBase64String();
            TokenType = "Bearer";
            ExpiresIn = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
            //Claims = securityToken.Claims;
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]

        public int ExpiresIn { get; set; }

        //[JsonIgnore]
        //public IEnumerable<Claim> Claims { get; set; }

    }
}
