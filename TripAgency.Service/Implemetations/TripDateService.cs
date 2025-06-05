using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class TripDateService : ReadAndAddService<TripDate, GetTripDateByIdDto, GetTripDatesDto, AddTripDateDto>, ITripDateService
    {
        private ITripDateRepositoryAsync _tripDateRepository { get; }
        private IMapper _mapper { get; }

        public TripDateService(ITripDateRepositoryAsync tripDateRepository,
                              IMapper mapper
                              ) : base(tripDateRepository, mapper)
        {
            _tripDateRepository = tripDateRepository;
            _mapper = mapper;
        }

        //public async Task<Result> UpdateStatusTripDate(UpdateTripDateDto updateTripDateDto)
        //{
        //   var tripDate= await _tripDateRepository.GetByIdAsync(updateTripDateDto.Id);
        //    if (tripDate is null)
        //        return Result.NotFound($"Not Found Trip Date with Id : {updateTripDateDto.Id}");
        //    tripDate.Status = updateTripDateDto.Status;

        //}
        //private (bool , string) CheckUpdateStatues(TripDataStatus NewStatus , TripDataStatus oldStatus)
        //{
        //    if(oldStatus is TripDataStatus.Completed)
        //    {
        //        switch (NewStatus)
        //        {
                    
        //            case TripDataStatus.Available:
        //                return (true, "");
        //            case TripDataStatus.Cancelled:
        //                return (true, "");
        //            case TripDataStatus.Completed:
        //                return (false, "The New Status Same The Old Status");
        //                default:
        //                return (false, "");
        //        };
        
        //    }
        //    else if (oldStatus is TripDataStatus.Cancelled)
        //    {
        //        return (false, "Cann't Update From Canscelled Status to any type else");

        //    }
        //    else if (oldStatus is TripDataStatus.Available)
        //    {
        //        switch (NewStatus)
        //        {

        //            case TripDataStatus.Available:
        //                return (true, "");
        //            case TripDataStatus.Cancelled:
        //                return (true, "");
        //            case TripDataStatus.Completed:
        //                return (false, "");
        //            default:
        //                return (false, "");
        //        };
        //    }
        //    else
        //    {
        //        return (false, "");

        //    }

        //}
    }
}
