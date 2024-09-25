using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class RequestStatusDto :BaseAuditableEntity
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public RequestStatusEnum? Status { get; set; }
        public ICollection<RequestStatusHistoryDto>? RequestStatusHistories { get; set; }

    }
}
