namespace ContractorsAuctioneer.Dtos
{
    public class UpdateRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public bool CanChangeOrder { get; set; }
        public int ClientId { get; set; }
        public int RegionId { get; set; }
        public DateTime EpireAt { get; set; }
    }
}
