namespace ContractorsAuctioneer.Entites
{
    public class VerificationCode : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime ExpiredTime { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
