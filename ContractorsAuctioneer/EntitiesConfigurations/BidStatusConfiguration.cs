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
        }


    }
}
