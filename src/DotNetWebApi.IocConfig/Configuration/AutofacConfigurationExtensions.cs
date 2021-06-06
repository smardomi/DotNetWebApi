using Autofac;
using DotNetWebApi.Common;
using DotNetWebApi.DataLayer.EFContext;
using DotNetWebApi.DataLayer.Repositories;
using DotNetWebApi.DomainClasses.Common;
using DotNetWebApi.Services.Jwt;
using Microsoft.AspNetCore.Http;

namespace DotNetWebApi.IocConfig.Configuration
{
    public static class AutofacConfigurationExtensions
    {
        public static void AddServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            containerBuilder.RegisterType<ApplicationDbContext>().As(typeof(IUnitOfWork)).InstancePerLifetimeScope();
            containerBuilder.RegisterType<HttpContextAccessor>().As(typeof(IHttpContextAccessor)).SingleInstance();

            var commonAssembly = typeof(SiteSettings).Assembly;
            var entitiesAssembly = typeof(IEntity).Assembly;
            var dataAssembly = typeof(ApplicationDbContext).Assembly;
            var servicesAssembly = typeof(TokenFactoryService).Assembly;
            var webFrameworkAssembly = typeof(AntiForgeryCookieService).Assembly;

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly, webFrameworkAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly, webFrameworkAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly, webFrameworkAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

    }
}
