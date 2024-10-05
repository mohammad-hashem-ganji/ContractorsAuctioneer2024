namespace ContractorsAuctioneer.Entites
{
    public class RequestRejectedByContractor : BaseAuditableEntity
    {
        public int Id{ get; set; }
        public int ContractorId { get; set; }
        public Contractor Contractor { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }
    }
}
