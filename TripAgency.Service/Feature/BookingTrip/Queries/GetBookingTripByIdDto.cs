using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.BookingTrip.Queries
{
    public class GetBookingTripByIdDto
    {
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal ActualPrice { get; set; }
        public string Notes { get; set; }
        public int TripDateId { get; set; }
        public int UserId { get; set; }

    }
    public class GetBookingTripForUserDto
    {
        public int BookingId { get; set; }
        public int PassengerCount { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ExpireTime { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal ActualPrice { get; set; }
        public string Notes { get; set; }
        public int UserId { get; set; }
        public GetPackageTripDateBookingDetailDto GetPackageTripDateBookingDetailDto { get; set; }
        public GetPaymentDto GetPaymentDto { get; set; }

    }
    public class GetPaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? TransactionRef { get; set; }
        public string PaymentMethodName { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentInstructions { get; set; }
        public bool CanCompletePayment { get; set; }
    }
    public class GetPackageTripDateBookingDetailDto
    {
        public int PackageTripdateId { get; set; }
        public string PackageTripName { get; set; }
        public PackageTripDateStatus PackageTripDateStatus { get; set; }
        public string ImageUrlPackageTrip {  get; set; }
        public DateTime StartTripDate { get; set; } 
        public DateTime EndTripDate { get; set; }
        public DateTime StartBookingTripDate { get; set; }
        public DateTime EndBookingTripDate { get; set; }
        public bool CanReviewTrip { get; set; }

    }
}
