using ContractorsAuctioneer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorsAuctioneer.EntitiesConfigurations
{
    public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            builder.HasKey(v => v.Id);
            builder.HasOne(v => v.ApplicationUser)
                .WithMany(a => a.VerificationCodes)
                .HasForeignKey(v => v.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
