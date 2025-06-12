using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class PackageTripDestinationService : WriteService<PackageTripDestination, AddPackageTripDestinationDto, UpdatePackageTripDestinationDto, GetPackageTripDestinationByIdDto>, IPackageTripDestinationService
    {
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; set; }
        private IDestinationRepositoryAsync _destinationRepositoryAsync { get; set; }
        private ITripDateRepositoryAsync _tripDateRepositoryAsync { get; }
        private ITripDestinationRepositoryAsync _tripDestinationRepositoryAsync { get; set; }
        private IPackageTripDestinationRepositoryAsync _packageTripDestinationRepoAsync { get; set; }
        private IMapper _mapper { get; }

        public PackageTripDestinationService(IPackageTripDestinationRepositoryAsync packageTripDestinationRepoAsync,
                                             IPackageTripRepositoryAsync packageTripRepositoryAsync,
                                             IMapper mapper,
                                             IDestinationRepositoryAsync destinationRepositoryAsync,
                                             ITripDateRepositoryAsync tripDateRepositoryAsync,
                                             ITripDestinationRepositoryAsync tripDestinationRepositoryAsync) : base(packageTripDestinationRepoAsync, mapper)
        {
            _packageTripDestinationRepoAsync = packageTripDestinationRepoAsync;
            _packageTripRepositoryAsync = packageTripRepositoryAsync;
            _mapper = mapper;
            _destinationRepositoryAsync = destinationRepositoryAsync;
            _tripDateRepositoryAsync = tripDateRepositoryAsync;
            _tripDestinationRepositoryAsync = tripDestinationRepositoryAsync;
        }
        public override async Task<Result<GetPackageTripDestinationByIdDto>> CreateAsync(AddPackageTripDestinationDto AddDto)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetByIdAsync(AddDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(AddDto.DestinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found Destination With Id : {AddDto.DestinationId}");
            }
            var TripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking().FirstOrDefaultAsync(x=>x.TripId == PackageTrip.TripId && x.DestinationId== Destination.Id);
            if (TripDestination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.BadRequest($" Cann't Add Destination With Id : {AddDto.DestinationId} For PackageTrip With Id : {AddDto.PackageTripId}");
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id && x.DestinationId == Destination.Id);
            if (PackageTripDestination is not  null)
            {
                return Result<GetPackageTripDestinationByIdDto>.BadRequest($"Destination Id : {AddDto.DestinationId} is already associated with Package Trip Id: {AddDto.PackageTripId}.");
            }
            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDestinationDto UpdateDto)
        {
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetByIdAsync(id);
            if(PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTripDestination With Id : {UpdateDto.Id}");

            }
            var PackageTrip = await _packageTripRepositoryAsync.GetByIdAsync(UpdateDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId}");
            }
            if (PackageTrip.Id != PackageTripDestination.PackageTripId)
            {
                return Result.BadRequest($"The PackageTrip With Id {PackageTrip.Id} Not Have PackgeTripDestinationId : {UpdateDto.Id}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(UpdateDto.DestinationId);
            if (Destination is null)
            {
                return Result.NotFound($"Not Found Destination With Id : {UpdateDto.PackageTripId}");
            }

            var TripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                       .FirstOrDefaultAsync(x => x.TripId == PackageTrip.TripId 
                                                                                              && x.DestinationId == Destination.Id);
            if (TripDestination is null)
            {
                return Result.BadRequest($" Cann't Add Destination With Id : {UpdateDto.PackageTripId} For PackageTrip With Id : {PackageTrip.Id}");
            }

            var CheckPackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id && x.DestinationId == Destination.Id);
            if (CheckPackageTripDestination is not null)
            {
                return Result.BadRequest($"Destination Id : {UpdateDto.DestinationId} is already associated with Package Trip Id: {UpdateDto.PackageTripId}.");
            }

            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                         .FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id);
            if (TripDate is not null)
            {
                return Result.BadRequest($" Cann't Update PackageTripDestination Because you have tripDate Connected before with PackageTrip: {PackageTrip.Id}");
            }
            return await base.UpdateAsync(id, UpdateDto);
        }
        public override async Task<Result> DeleteAsync(int id)
        {
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetByIdAsync(id);
            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTripDestination With Id : {id}");

            }
            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.PackageTripId == PackageTripDestination.PackageTripId);
            if (TripDate is not null)
            {
                return Result.BadRequest($" Cann't Delete PackageTripDestination Because you have TripDate :{TripDate.Id} Connected before with PackageTrip: {PackageTripDestination.PackageTripId}");

            }
            return await base.DeleteAsync(id);
        }

    }
}
