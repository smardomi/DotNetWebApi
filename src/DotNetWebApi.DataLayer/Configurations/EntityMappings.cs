using Microsoft.EntityFrameworkCore;

namespace DotNetWebApi.DataLayer.Configurations
{
    public static class EntityMappings
    {
        /// <summary>
        /// Adds all of the ASP.NET Core Identity related mappings at once.
       /// </summary>
        public static void AddCustomEntityMappings(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityMappings).Assembly);
        }
    }
}