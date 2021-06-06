using System.Collections.Generic;
using System.Security.Claims;

namespace DotNetWebApi.IocConfig.Configuration
{
    public interface IAntiForgeryCookieService
    {
        void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims);
        void DeleteAntiForgeryCookies();
    }
}
