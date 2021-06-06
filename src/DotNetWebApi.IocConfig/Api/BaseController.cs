using DotNetWebApi.Common.Pagination;
using DotNetWebApi.IocConfig.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebApi.IocConfig.Api
{
    [ApiController]
    //[AllowAnonymous]
    [ApiResultFilter]
    //[EnableCors("CorsPolicy")]
    [Route("api/v{version:apiVersion}/[controller]")]// api/v1/[controller]
    public class BaseController : ControllerBase
    {
        //public UserRepository UserRepository { get; set; } => property injection
        public bool UserIsAuthenticated => HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated;
        
        public record UrlQueryPagingParameters(int Limit = 50, int Page = 1);

        protected dynamic GeneratePageLinks(UrlQueryPagingParameters
                queryParameters,
            dynamic response,
            string routeName)
        {
            if (response.CurrentPage > 1)
            {
                var prevRoute = Url.RouteUrl(routeName, new { limit = queryParameters.Limit, page = queryParameters.Page - 1 }, Request.Scheme);

                response.Links.Add(LinkedResourceType.Prev, prevRoute);

            }

            if (response.CurrentPage < response.TotalPages)
            {
                var nextRoute = Url.RouteUrl(routeName, new { limit = queryParameters.Limit, page = queryParameters.Page + 1 }, Request.Scheme);

                response.Links.Add(LinkedResourceType.Next, nextRoute);
            }

            return response;
        }
    }
}
