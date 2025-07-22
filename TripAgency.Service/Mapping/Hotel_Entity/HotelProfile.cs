using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.Hotel_Entity
{
    public partial class HotelProfile : Profile
    {
        public HotelProfile()
        {
            GetHotelByIdMapping();
            GetHotelsMapping();
            AddHotelMapping();
            EditHotelMapping();
        }
    }
}
