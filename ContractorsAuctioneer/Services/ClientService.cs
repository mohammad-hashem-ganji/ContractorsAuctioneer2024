using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace ContractorsAuctioneer.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public ClientService(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<int> AddAsync(Client client, CancellationToken cancellationToken)
        {
            //client.ApplicationUser = _authService.RegisterAsync() 
            await _context.Clients.AddAsync(client, cancellationToken);
            var trackeNum = await _context.SaveChangesAsync(cancellationToken);
            if (trackeNum >=1)
            {
                return client.Id;
            }
            else
            {
                return 0;
            }
            
        }
        public async Task<Result<ClientDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var client = await _context.Clients
                  .Where(x => x.Id == id)
                  .Include(x => x.Requests)
                  .FirstOrDefaultAsync(cancellationToken);
                if (client is null)
                {
                    return new Result<ClientDto>().WithValue(null).Failure(ErrorMessages.ClientNotFound);
                }
                else
                {
                    var clientDto = new ClientDto
                    {
                        Id = client.Id,
                        address = client.address,
                        ApplicationUserId = client.ApplicationUserId,
                        CreatedAt = client.CreatedAt,
                        CreatedBy = client.CreatedBy,
                        IsDeleted = client.IsDeleted,
                        DeletedAt = client.DeletedAt,
                        DeletedBy = client.DeletedBy,
                        LicensePlate = client.LicensePlate,
                        MainSection = client.MainSection,
                        MobileNubmer = client.MobileNubmer,
                        NCcode = client.NCcode,
                        PostalCode = client.PostalCode,
                        SubSection = client.SubSection,
                        Requests = client.Requests.Select(rs => new RequestDto
                        {
                            Id = rs.Id,
                            Title = rs.Title,
                            ConfirmationDate = rs.ConfirmationDate,
                            RegistrationDate = rs.RegistrationDate,
                            CreatedAt = rs.CreatedAt,
                            CreatedBy = rs.CreatedBy,
                            Description = rs.Description,
                            IsDeleted = rs.IsDeleted,
                            DeletedAt = rs.DeletedAt,
                            DeletedBy = rs.DeletedBy,
                            CanChangeOrder = rs.CanChangeOrder,
                            UpdatedAt = rs.UpdatedAt,
                            UpdatedBy = rs.UpdatedBy

                        }).ToList(),
                        UpdatedAt = client.UpdatedAt,
                        UpdatedBy = client.UpdatedBy,
                    };
                    return new Result<ClientDto>().WithValue(clientDto).Success(SuccessMessages.ClientFound);
                }
            }
            catch (Exception ex)
            {
                return new Result<ClientDto>().WithValue(null).Failure(ex.Message);
            }
        }
    }
}
