using DotNetWebApi.Common;

namespace DotNetWebApi.Services.DataInitializer
{
    public interface IDataInitializer : IScopedDependency
    {
        void InitializeData();
    }
}
