﻿using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddClientDto : BaseAddAuditableDto
    {
        public string? NCcode { get; set; }
        public string? MobileNubmer { get; set; }
        public string? PostalCode { get; set; }
        public string? LicensePlate { get; set; }
        public string? address { get; set; }
        public string? MainSection { get; set; }
        public string? SubSection { get; set; }
    }
}
