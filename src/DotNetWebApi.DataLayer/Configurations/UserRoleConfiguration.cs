using DotNetWebApi.DomainClasses.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetWebApi.DataLayer.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.Property(a => a.RoleId).HasColumnName("ROLE_SEQ");
            builder.Property(a => a.UserId).HasColumnName("USER_SEQ");
            builder.ToTable("GEN_WEB_API_USER_ROLES");

            builder.HasOne(userRole => userRole.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(userRole => userRole.RoleId);

            builder.HasOne(userRole => userRole.User)
                .WithMany(user => user.Roles)
                .HasForeignKey(userRole => userRole.UserId);
        }
    }
}