using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DotNetWebApi.DataLayer.EFContext
{

    public interface IUnitOfWork : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        int ExecuteSqlRaw(string query, params object[] parameters);
        Task<int> ExecuteSqlRawAsync(string query, CancellationToken cancellationToken = new CancellationToken(), params object[] parameters);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}