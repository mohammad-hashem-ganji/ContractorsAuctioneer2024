﻿using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class RegionDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? ContractorSystemCode { get; set; } //******** datatype?
        public ICollection<RequestDto>? Requests { get; set; }
    }
}
