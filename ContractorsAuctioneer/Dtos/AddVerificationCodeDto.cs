namespace ContractorsAuctioneer.Dtos
{
    public class AddVerificationCodeDto
    {
        public string Token { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
