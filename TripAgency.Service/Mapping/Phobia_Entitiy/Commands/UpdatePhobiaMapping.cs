using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Phobia.Commands;

namespace TripAgency.Service.Mapping.Phobia_Entity
{
    public partial class PhobiaProfile
    {
        public void UpdatePhobiaMapping()
        {
            CreateMap<UpdatePhobiaDto, Phobia>().
               ForMember(d => d.Id, op => op.MapFrom(s => s.Id)).
               ForMember(d => d.Name, op => op.MapFrom(s => s.Name)).
               ForMember(d => d.Description, op => op.MapFrom(s => s.Description));
        }
    }
}
