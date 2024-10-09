namespace ContractorsAuctioneer.Dtos
{
    public class BaseUpdateAuditableDto
    {
        public DateTime? UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
