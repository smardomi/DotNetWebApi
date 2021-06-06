using DotNetWebApi.DomainClasses.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetWebApi.DataLayer.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("SEQ");
            builder.Property(a => a.ConcurrencyStamp).HasColumnName("CONCURRENCY_STAMP").HasColumnType("VARCHAR(250)");
            builder.Property(a => a.Name).HasColumnName("NAME").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(a => a.NormalizedName).HasColumnName("NORMALIZED_NAME").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(a => a.Description).HasColumnName("DESCRIPTION").HasColumnType("VARCHAR(250)");
            builder.ToTable("GEN_WEB_API_ROLES");
        }
    }
}