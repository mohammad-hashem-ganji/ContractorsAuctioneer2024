﻿using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace ContractorsAuctioneer.Dtos
{
    public class AddContractorDto : BaseAddAuditableDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? LandlineNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ApplicationUserId { get; set; }

    }
}
