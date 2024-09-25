using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddRequestDto : BaseAuditableEntity
    {
        public string Title { get; set; }
        public string RequestNumber { get; set; }
        public string Description { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public bool CanChangeOrder { get; set; }
        public ClientDto Client { get; set; }
        public RegionDto Region { get; set; }


    }
}
