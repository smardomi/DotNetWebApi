using AutoMapper;

namespace DotNetWebApi.IocConfig.CustomMapping
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile profile);
    }
}
