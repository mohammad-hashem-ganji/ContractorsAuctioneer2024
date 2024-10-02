namespace ContractorsAuctioneer.Entites
{
    public class BaseAddAuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}
