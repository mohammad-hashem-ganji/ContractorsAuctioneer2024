using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddRequestStatusHistoryDto : BaseAddAuditableDto
    {
        public int RequetStatusId { get; set; }
        public RequestStatusEnum? Status { get; set; }
    }
}
