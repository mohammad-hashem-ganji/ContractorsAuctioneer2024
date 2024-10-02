namespace ContractorsAuctioneer.Entites
{
    public class RejectedRequest :BaseAddAuditableEntity
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }
    }
}
