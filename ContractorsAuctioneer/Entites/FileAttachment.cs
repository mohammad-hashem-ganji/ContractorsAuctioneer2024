﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class FileAttachment : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int RequestId { get; set; }
        public Request? Request { get; set; }
    }
}
