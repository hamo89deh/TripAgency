using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TripReview;

namespace TripAgency.Service.Mapping.TripReview_Entity
{
    public partial class TripReviewProfile
    {
        public void GetTripReviewsMapping()
        {

            CreateMap<TripReview, GetTripReviewsDto>().
                 ForMember(d => d.Id, op => op.MapFrom(s => s.Id)).
                 ForMember(d => d.PackageTripDateId, op => op.MapFrom(s => s.PackageTripDateId)).
                 ForMember(d => d.Rating, op => op.MapFrom(s => s.Rating)).
                 ForMember(d => d.Comment, op => op.MapFrom(s => s.Comment)).
                 ForMember(d => d.UserId, op => op.MapFrom(s => s.UserId));
        }
    }
}
