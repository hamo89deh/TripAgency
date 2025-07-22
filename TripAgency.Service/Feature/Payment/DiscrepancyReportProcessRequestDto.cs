using System.ComponentModel.DataAnnotations;
using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.Payment
{
    public class DiscrepancyReportProcessRequestDto
    {
        public int ReportId { get; set; }
        public PaymentDiscrepancyStatusEnum Status { get; set; } // الحالة الجديدة للبلاغ
        public decimal VerifiedAmount { get; set; }
        public string? AdminNotes { get; set; } // ملاحظات المسؤول (اختياري)
    }
}
