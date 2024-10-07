﻿using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using Azure.Core;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace ContractorsAuctioneer.Services
{
    public class BidOfContractorCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBidStatusService _bidStatusService;

        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);
        public BidOfContractorCheckService(IServiceProvider serviceProvider, IBidStatusService bidStatusService)
        {
            _serviceProvider = serviceProvider;
            _bidStatusService = bidStatusService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckBidOfContractorsAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
        private async Task CheckBidOfContractorsAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    var xpiredbidsForClients = await dbContext.BidOfContractors
                        .Where(c => (c.ExpireAt.HasValue && c.ExpireAt <= DateTime.Now) ||
                            (c.BidStatuses != null && c.BidStatuses
                            .Any(x => x.Status != BidStatusEnum.BidApprovedByClient 
                            || x.Status != BidStatusEnum.TimeForCheckingBidForClientExpired)))
                            .ToArrayAsync(stoppingToken);

                    foreach (var bid in xpiredbidsForClients)
                    {

                        bid.ExpireAt = null;
                        var expired = await _bidStatusService
    
                        .AddAsync(new Dtos.AddBidStatusDto
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = 100,
                            BidOfContractorId = bid.Id,
                            Status = BidStatusEnum.TimeForCheckingBidForClientExpired,
                        }, stoppingToken);
                        if (expired.IsSuccessful)
                        {
                            dbContext.BidOfContractors.Update(bid);
                        }  
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}

