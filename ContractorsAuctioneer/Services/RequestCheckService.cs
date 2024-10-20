
using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class RequestCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(3);

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
                        .Where(r => (r.ExpireAt.HasValue && r.ExpireAt <= DateTime.Now) &&
                        r.RequestStatuses != null && r.RequestStatuses
                        .Any(x => x.Status == RequestStatusEnum.RequestRejectedByClient))
                        .ToArrayAsync(stoppingToken);
                    var a = " ";
                    var requestsAfterTenderExpieredAndAcceptedByClient = await dbContext.Requests
                        .Where(r => r.ExpireAt.HasValue
                        && r.ExpireAt <= DateTime.Now
                        && r.RequestStatuses != null
                        && r.RequestStatuses
                        .Any(x => x.Status == RequestStatusEnum.RequestApprovedByClient))
                        .ToArrayAsync(stoppingToken);
                    var requestsAfterTenderExpieredAndNOotCheckedByClient = await dbContext.Requests
                       .Where(r => r.ExpireAt.HasValue
                       && r.ExpireAt <= DateTime.Now
                       && r.RequestStatuses == null)
                       .ToArrayAsync(stoppingToken);
                    foreach (var request in rejectedRequestByClient)
                    {
                        request.IsActive = false;

                        request.ExpireAt = null;
                        var tenderOver = await requestStatusService
                         .AddAsync(new Dtos.AddRequestStatusDto
                         {
                             RequestId = request.Id,
                             Status = RequestStatusEnum.TimeForCheckingBidForClientExpired,
                             CreatedBy = 100
                         }, stoppingToken);
                        if (tenderOver.IsSuccessful)
                        {
                            dbContext.Requests.Update(request);
                            var bids = dbContext.BidOfContractors.Where(x => x.RequestId == request.Id).ToListAsync(stoppingToken);
                            bids.Result.ForEach(x => x.ExpireAt = DateTime.Now.AddDays(3));
                            dbContext.BidOfContractors.UpdateRange(bids.Result);
                        }
                    }
                    foreach (var request in requestsAfterTenderExpieredAndAcceptedByClient)
                    {
                        request.IsActive = true;

                        request.ExpireAt = null;
                        var tenderOver = await requestStatusService
                            .AddAsync(new Dtos.AddRequestStatusDto
                            {
                                RequestId = request.Id,
                                Status = RequestStatusEnum.TimeForCheckingBidForClientExpired,
                                CreatedBy = 100
                            }, stoppingToken);
                        if (tenderOver.IsSuccessful)
                        {
                            dbContext.Requests.Update(request);
                            var bids = dbContext.BidOfContractors.Where(x => x.RequestId == request.Id).ToListAsync(stoppingToken);
                            bids.Result.ForEach(x => x.ExpireAt = DateTime.Now.AddDays(3));
                            dbContext.BidOfContractors.UpdateRange(bids.Result);
                        }
                    }
                    foreach (var request in requestsAfterTenderExpieredAndNOotCheckedByClient)
                    {
                        request.IsActive = false;
                        request.IsTenderOver = true;
                        request.ExpireAt = null;
                        var tenderOver = await requestStatusService
                            .AddAsync(new Dtos.AddRequestStatusDto
                            {
                                RequestId = request.Id,
                                Status = RequestStatusEnum.TimeForCheckingBidForClientExpired,
                                CreatedBy = 100
                            }, stoppingToken);
                        if (tenderOver.IsSuccessful)
                        {
                            dbContext.Requests.Update(request);
                            var bids = dbContext.BidOfContractors.Where(x => x.RequestId == request.Id).ToListAsync(stoppingToken);
                            bids.Result.ForEach(x => x.ExpireAt = DateTime.Now.AddDays(3));
                            dbContext.BidOfContractors.UpdateRange(bids.Result);
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
