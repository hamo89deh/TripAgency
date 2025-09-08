using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.Offer_Entity
{
    public partial class OfferProfile : Profile 
    {
        public OfferProfile()
        {
            GetOffersMapping();
            GetOfferByIdMapping();

        }
    }
}
