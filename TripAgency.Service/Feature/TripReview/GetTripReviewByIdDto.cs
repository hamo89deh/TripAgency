namespace TripAgency.Service.Feature.TripReview
{
    public class GetTripReviewByIdDto
    {
        public int Id { get; set; }
        public int PackageTripDateId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } // اسم المستخدم للعرض
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
