﻿
using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class RequestCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public RequestCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckRequestsAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckRequestsAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var requestStatusService = scope.ServiceProvider.GetRequiredService<IRequestStatusService>();

                try
                {
                    var rejectedRequestByClient = await dbContext.Requests
                        .Where(r => (r.ExpireAt.HasValue && r.ExpireAt <= DateTime.Now) ||
                        (r.RequestStatuses != null && r.RequestStatuses
                        .Any(x => x.Status == RequestStatusEnum.RequestRejectedByClient)))
                        .ToArrayAsync(stoppingToken);

                    var requestsAfterTenderExpiered = await dbContext.Requests
                        .Where(r => r.ExpireAt.HasValue
                        && r.ExpireAt <= DateTime.Now
                        && r.RequestStatuses != null
                        && r.RequestStatuses
                        .Any(x => x.Status == RequestStatusEnum.RequestApprovedByClient))
                        .ToArrayAsync(stoppingToken);

                    foreach (var request in rejectedRequestByClient)
                    {
                        request.IsActive = false;
                        request.IsTenderOver = true;
                        request.ExpireAt = null;
                        dbContext.Requests.Update(request);
                    }
                    foreach (var request in requestsAfterTenderExpiered)
                    {
                        request.IsActive = true;
                        request.IsTenderOver = true;
                        request.ExpireAt = null;
                        var tenderOver = await requestStatusService
                            .AddAsync(new Dtos.AddRequestStatusDto
                            {
                                RequestId = request.Id,
                                Status = RequestStatusEnum.RequestTenderFinished,
                            }, stoppingToken);
                        if (tenderOver.IsSuccessful)
                        {
                            dbContext.Requests.Update(request);
                        }                
                    }
                    // Do somthing for rejectedRequestByContractor !! ***
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
