using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.Activity_Entity
{
    public partial class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            GetActivitiesMapping();
            GetActivityByIdMapping();
            AddActivityMapping();
            UpdateActivityMapping();
        }
    }
}
