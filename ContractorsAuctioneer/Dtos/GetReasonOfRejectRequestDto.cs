﻿using ContractorsAuctioneer.Entites;
using System.ComponentModel.DataAnnotations;

namespace ContractorsAuctioneer.Dtos
{
    public class GetReasonOfRejectRequestDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Comment { get; set; }
        public RejectReasonEnum? Reason { get; set; }
        public DateTime DateRejected { get; set; }
        public int UserId { get; set; }
    }
}
