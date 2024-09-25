using ContractorsAuctioneer.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.EntitiesConfigurations
{
    public class BidStatusConfiguration : IEntityTypeConfiguration<BidStatus>
    {
        public void Configure(EntityTypeBuilder<BidStatus> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasMany(b => b.BidStatusHistories)
                .WithOne(b => b.BidStatus)
                .HasForeignKey(x => x.BidStatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }


    }
}
