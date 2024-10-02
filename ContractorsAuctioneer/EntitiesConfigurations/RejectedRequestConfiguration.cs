using ContractorsAuctioneer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorsAuctioneer.EntitiesConfigurations
{
    public class RejectedRequestConfiguration : IEntityTypeConfiguration<RejectedRequest>
    {
        public void Configure(EntityTypeBuilder<RejectedRequest> builder)
        {
            builder.HasKey(rr => rr.Id);

            builder.Property(rr => rr.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(r => r.Request)
                .WithMany(r => r.RejectedRequests)
                .HasForeignKey(r => r.RequestId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
