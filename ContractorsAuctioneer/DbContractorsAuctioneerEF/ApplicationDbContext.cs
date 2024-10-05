using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.EntitiesConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RequestConfiguration());
            modelBuilder.ApplyConfiguration(new RequestStatusConfiguration());
            modelBuilder.ApplyConfiguration(new RequestStatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ContractorBidConfiguration());
            modelBuilder.ApplyConfiguration(new BidStatusConfiguration());
            modelBuilder.ApplyConfiguration(new BidStatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectStatusConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectStatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new RegionConfiguration());
            modelBuilder.ApplyConfiguration(new ContractorConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new FileAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new LastLoginHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new RejectedRequestConfiguration());
            base.OnModelCreating(modelBuilder);
            List<IdentityRole<int>> roles = new List<IdentityRole<int>>
            {
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = "Client",
                    NormalizedName = "CLIENT"
                },
                new IdentityRole<int>
                {
                    Id=2,
                    Name = "Contractor",
                    NormalizedName = "CONTRACTOR"
                },
                new IdentityRole<int>
                { 
                    Id= 3,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
            };
            modelBuilder.Entity<IdentityRole<int>>().HasData(roles);
            
        }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<BidOfContractor> BidOfContractors { get; set; }
        public DbSet<BidStatus> BidStatuses { get; set; }
        public DbSet<BidStatusHistory> BidStatusHistories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<ProjectStatusHistory> ProjectStatusHistories { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<LastLoginHistory> LastLoginHistories{ get; set; }
        public DbSet<RejectedRequest> RejectedRequests{ get; set; }
        public DbSet<RequestRejectedByContractor> RequestRejectedByContractors{ get; set; }
    }

}
