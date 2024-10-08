using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddRequestStatusDto : BaseAddAuditableDto
    {
        public int RequestId { get; set; }
        public RequestStatusEnum Status { get; set; }

    }

}
