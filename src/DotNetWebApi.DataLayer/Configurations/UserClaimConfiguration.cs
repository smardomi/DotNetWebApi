using DotNetWebApi.DomainClasses.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetWebApi.DataLayer.Configurations
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("SEQ");
            builder.Property(a => a.UserId).HasColumnName("USER_SEQ");
            builder.Property(a => a.ClaimType).HasColumnName("CLAIM_TYPE").HasColumnType("VARCHAR(200)");
            builder.Property(a => a.ClaimValue).HasColumnName("CLAIM_VALUE").HasColumnType("VARCHAR(300)");
            builder.ToTable("GEN_WEB_API_USER_CLAIMS");

            builder.HasOne(userClaim => userClaim.User)
                   .WithMany(user => user.Claims)
                   .HasForeignKey(userClaim => userClaim.UserId);
        }
    }
}