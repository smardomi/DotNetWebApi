using DotNetWebApi.DomainClasses.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetWebApi.DataLayer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("SEQ");
            builder.Property(a => a.FullName).HasColumnName("FULL_NAME").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(a => a.IsActive).HasColumnName("IS_ACTIVE").IsRequired();
            builder.Property(a => a.UserName).HasColumnName("USERNAME").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(a => a.NormalizedUserName).HasColumnName("NORMALIZED_USERNAME").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(a => a.Email).HasColumnName("EMAIL").HasColumnType("VARCHAR(200)");
            builder.Property(a => a.NormalizedEmail).HasColumnName("NORMALIZED_EMAIL").HasColumnType("VARCHAR(200)");
            builder.Property(a => a.EmailConfirmed).HasColumnName("EMAIL_CONFIRMED");
            builder.Property(a => a.PasswordHash).HasColumnName("PASSWORD_HASH").HasColumnType("VARCHAR(500)");
            builder.Property(a => a.SecurityStamp).HasColumnName("SECURITY_STAMP").HasColumnType("VARCHAR(500)");
            builder.Property(a => a.ConcurrencyStamp).HasColumnName("CONCURRENCY_STAMP").HasColumnType("VARCHAR(250)");
            builder.Property(a => a.PhoneNumber).HasColumnName("PHONE_NUMBER").HasColumnType("VARCHAR(50)");
            builder.Property(a => a.PhoneNumberConfirmed).HasColumnName("PHONE_NUMBER_CONFIRMED");
            builder.Property(a => a.TwoFactorEnabled).HasColumnName("TWO_FACTOR_ENABLED");
            builder.Property(a => a.LockoutEnd).HasColumnName("LOCK_OUT_END");
            builder.Property(a => a.LockoutEnabled).HasColumnName("LOCK_OUT_ENABLED");
            builder.Property(a => a.AccessFailedCount).HasColumnName("ACCESS_FAILED_COUNT").HasColumnType("NUMBER(10)");
            builder.Property(a => a.LastLoginDate).HasColumnName("LAST_LOGIN_DATE").HasColumnType("DATE");
            builder.ToTable("GEN_WEB_API_USERS");
        }
    }
}