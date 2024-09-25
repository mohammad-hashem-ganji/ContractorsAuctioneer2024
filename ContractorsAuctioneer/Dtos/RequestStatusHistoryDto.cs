namespace ContractorsAuctioneer.Dtos
{
    public class RequestStatusHistoryDto
    {
        public int Id { get; set; }
        public int RequestStatusId { get; set; }
        public RequestStatusDto? RequestStatus { get; set; }

    }
}
