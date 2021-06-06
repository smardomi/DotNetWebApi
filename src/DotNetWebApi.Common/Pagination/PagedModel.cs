using System.Collections.Generic;

namespace DotNetWebApi.Common.Pagination
{
    public class PagedModel<TModel> : ILinkedResource
    {
        public PagedModel()
        {
            Items = new List<TModel>();
            Links = new Dictionary<LinkedResourceType, string>();
        }

        const int MaxPageSize = 500;
        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<TModel> Items { get; set; }
        public IDictionary<LinkedResourceType, string> Links { get; set; }
    }

    public enum LinkedResourceType
    {
        None,
        Prev,
        Next
    }
}
