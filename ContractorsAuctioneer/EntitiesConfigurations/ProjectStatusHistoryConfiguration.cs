

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
    public class ProjectStatusHistoryConfiguration : IEntityTypeConfiguration<ProjectStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ProjectStatusHistory> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
