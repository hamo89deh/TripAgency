using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.Phobia_Entity
{
    public partial class PhobiaProfile : Profile
    {
        public PhobiaProfile()
        {
            GetPhobiaByIdMapping();
            GetPhobiasMapping();
            AddPhobiaMapping();
            UpdatePhobiaMapping();
        }
    }
}
