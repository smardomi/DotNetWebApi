using DotNetWebApi.DomainClasses.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetWebApi.DataLayer.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("SEQ");
            builder.Property(a => a.Token).HasColumnName("REFRESH_TOKEN").HasColumnType("VARCHAR(500)");
            builder.Property(a => a.AccessToken).HasColumnName("ACCESS_TOKEN").HasColumnType("VARCHAR(500)").IsRequired();
            builder.Property(a => a.IsUsed).HasColumnName("IS_USED").IsRequired();
            builder.Property(a => a.IsRevoked).HasColumnName("IS_REVOKED").IsRequired();
            builder.Property(a => a.CreateDate).HasColumnName("CREATE_DATE").HasColumnType("DATE").IsRequired();
            builder.Property(a => a.ExpiryDate).HasColumnName("EXPIRY_DATE").HasColumnType("DATE").IsRequired();
            builder.ToTable("GEN_WEB_API_REFRESH_TOKENS");


            builder.HasOne(p => p.User).WithMany(c => c.RefreshTokens).HasForeignKey(p => p.UserId);

        }
    }
}