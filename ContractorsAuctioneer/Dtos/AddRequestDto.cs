﻿using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddRequestDto 
    {
        public string Title { get; set; }
        public string RequestNumber { get; set; }
        public string Description { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string NCode { get; set; }
        public string PhoneNumber { get; set; }
        public AddClientDto Client { get; set; }
        public AddRegionDto Region { get; set; }
        


    }
}
