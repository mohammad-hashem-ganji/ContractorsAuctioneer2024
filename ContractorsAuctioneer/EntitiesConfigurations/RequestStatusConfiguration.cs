

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
    public class RequestStatusConfiguration : IEntityTypeConfiguration<RequestStatus>
    {
        public void Configure(EntityTypeBuilder<RequestStatus> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasMany(r => r.RequestStatusHistories)
                .WithOne(r => r.RequestStatus)
                .HasForeignKey(x => x.RequestStatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
