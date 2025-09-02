using TripAgency.Data.Entities.Identity;

namespace TripAgency.Data.Entities
{
    public class TripReview
    {
        public int Id { get; set; }
        public int PackageTripDateId { get; set; }
        public int UserId { get; set; } // معرف المستخدم
        public int Rating { get; set; } // التقييم (1-5)
        public string Comment { get; set; } // التعليق (اختياري)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // العلاقات
        public PackageTripDate PackageTripDate { get; set; }
        public User User { get; set; }
    }

}
