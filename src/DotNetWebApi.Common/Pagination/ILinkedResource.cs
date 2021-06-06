using System.Collections.Generic;

namespace DotNetWebApi.Common.Pagination
{
    public interface ILinkedResource
    {
        IDictionary<LinkedResourceType, string> Links { get; set; }
    }
}
