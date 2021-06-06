using DotNetWebApi.Common;
using Swashbuckle.AspNetCore.Filters;

namespace DotNetWebApi.IocConfig.Api
{
    public class ResponseTypes400 : IExamplesProvider<ApiResult>
    {
        public ApiResult GetExamples()
        {
            return new ApiResult(false, ApiResultStatusCode.BadRequest);
        }
    }
}
