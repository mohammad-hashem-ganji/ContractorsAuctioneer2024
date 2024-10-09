using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class RequestStatusDto : BaseAddAuditableDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public RequestStatusEnum? Status { get; set; }

    }
}
