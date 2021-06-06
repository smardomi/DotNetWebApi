using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetWebApi.Common.Utilities;
using DotNetWebApi.DataLayer.Configurations;
using DotNetWebApi.DomainClasses.Common;
using DotNetWebApi.DomainClasses.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetWebApi.DataLayer.EFContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int,UserClaim,UserRole,UserLogin,RoleClaim,UserToken>, IUnitOfWork
    //DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MyApiDb;Integrated Security=true");
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddCustomEntityMappings();

            var entitiesAssembly = typeof(IEntity).Assembly;

            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddPluralizingTableNameConvention();
        }

        public override int SaveChanges()
        {
            CleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            CleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            CleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CleanString();
            return base.SaveChangesAsync(cancellationToken);
        }

        public int ExecuteSqlRaw(string query, params object[] parameters)
        {
            return Database.ExecuteSqlRaw(query, parameters);
        }

        public async Task<int> ExecuteSqlRawAsync(string query, CancellationToken cancellationToken = new CancellationToken(), params object[] parameters)
        {
            return await Database.ExecuteSqlRawAsync(query, parameters,cancellationToken);
        }

        //public static List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        //{
        //    using (var context  = new DbContext())
        //    {
        //        using (var command = context.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = query;
        //            command.CommandType = CommandType.Text;

        //            context.Database.OpenConnection();

        //            using (var result = command.ExecuteReader())
        //            {
        //                var entities = new List<T>();

        //                while (result.Read())
        //                {
        //                    entities.Add(map(result));
        //                }

        //                return entities;
        //            }
        //        }
        //    }
        //}
        private void CleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }
    }
}
