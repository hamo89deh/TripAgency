using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.Promotioin_Entity
{
    public partial class PromotionProfile : Profile 
    {
        public PromotionProfile()
        {
            GetPromotionsMapping();
            GetPromotionByIdMapping();

        }
    }
}
