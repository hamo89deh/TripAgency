using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Activity.Queries;

namespace TripAgency.Service.Mapping.Activity_Entity
{
    public partial class ActivityProfile
    {
        public void GetActivitiesMapping()
        {
            CreateMap<Activity, GetActivitiesDto>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.Description))
                .ForMember(d => d.Price, op => op.MapFrom(s => s.Price));

        }
    }

}
