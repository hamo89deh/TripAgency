using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.TripReview
{
    public class AddTripReviewDto
    {
        public int PackageTripDateId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
