using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Phobia.Commands;
using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Service.Mapping.Phobia_Entity
{
    public partial class PhobiaProfile
    {
        public void AddPhobiaMapping()
        {
            CreateMap<AddPhobiaDto, Phobia>().
               ForMember(d => d.Name, op => op.MapFrom(s => s.Name)).
               ForMember(d => d.Description, op => op.MapFrom(s => s.Description));
        }
    }
}
