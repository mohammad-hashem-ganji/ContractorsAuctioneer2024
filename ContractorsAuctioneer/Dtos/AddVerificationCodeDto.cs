namespace ContractorsAuctioneer.Dtos
{
    public class AddVerificationCodeDto
    {
        public string Code { get; set; }
        public DateTime ExpiredTime { get; set; }
        public int ApplicationUserId { get; set; }
    }
}
