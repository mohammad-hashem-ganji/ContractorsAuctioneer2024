namespace ContractorsAuctioneer.Dtos
{
    public class GetVerificationCodeDto
    {
        public string Code { get; set; }
        public int ApplicationUserId { get; set; }
        public DateTime ExpiredTime { get; set; }
    }
}
