using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.TripReview_Entity
{
    public partial class TripReviewProfile : Profile
    {
        public TripReviewProfile()
        {
            GetTripReviewByIdMapping();
            GetTripReviewsMapping();
        }
    }
}
