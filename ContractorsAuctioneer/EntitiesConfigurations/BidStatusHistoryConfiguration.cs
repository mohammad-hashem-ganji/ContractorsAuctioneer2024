

using ContractorsAuctioneer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.EntitiesConfigurations
{
    public class BidStatusHistoryConfiguration : IEntityTypeConfiguration<BidStatusHistory>
    {
        public void Configure(EntityTypeBuilder<BidStatusHistory> builder)
        {
            builder.HasKey(b => b.Id);
        }
    }
}
