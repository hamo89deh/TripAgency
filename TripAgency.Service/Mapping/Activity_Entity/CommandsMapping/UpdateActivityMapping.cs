using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Activity.Commands;

namespace TripAgency.Service.Mapping.Activity_Entity
{
    public partial class ActivityProfile
    {
        public void UpdateActivityMapping()
        {
            CreateMap<UpdateActivityDto, Activity>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.Description))
                .ForMember(d => d.Price, op => op.MapFrom(s => s.Price));

        }
    }
}
