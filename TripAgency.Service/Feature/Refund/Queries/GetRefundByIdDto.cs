using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.Refund.Queries
{
    public class GetRefundByIdDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public RefundStatus Status { get; set; }
        public string TransactionReference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? PaymentId { get; set; } 
        public int? ReportId { get; set; }    

        public DateTime? RefundProcessedDate { get; set; } 
        public string? AdminNotes { get; set; } 
        public string? TransactionRefunded { get; set; }  

    }
}
