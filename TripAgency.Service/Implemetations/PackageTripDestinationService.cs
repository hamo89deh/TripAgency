using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                               .FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id 
                                                                                                      && x.DestinationId == Destination.Id);
            if (PackageTripDestination is not  null)
            {
                return Result<GetPackageTripDestinationByIdDto>.BadRequest($"Destination Id : {AddDto.DestinationId} is already associated with Package Trip Id: {AddDto.PackageTripId}.");
            }

            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDestinationDto UpdateDto)
        {
           
            var PackageTrip = await _packageTripRepositoryAsync.GetByIdAsync(UpdateDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId}");
            }         
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(UpdateDto.DestinationId);
            if (Destination is null)
            {
                return Result.NotFound($"Not Found Destination With Id : {UpdateDto.PackageTripId}");
            }

            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                              .FirstOrDefaultAsync(p => p.PackageTripId == UpdateDto.PackageTripId
                                                                                                   && p.DestinationId == UpdateDto.DestinationId);
            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId} and Destination id  {UpdateDto.DestinationId}");

            }

            var TripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                       .FirstOrDefaultAsync(x => x.TripId == PackageTrip.TripId 
                                                                                              && x.DestinationId == Destination.Id);
            if (TripDestination is null)
            {
                return Result.BadRequest($" Cann't Add Destination With Id : {UpdateDto.PackageTripId} For PackageTrip With Id : {PackageTrip.Id}");
            }

          
            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                         .FirstOrDefaultAsync(x => x.PackageTripId == PackageTrip.Id);
            if (TripDate is not null)
            {
                return Result.BadRequest($" Cann't Update PackageTripDestination Because you have tripDate associated with PackageTrip: {PackageTrip.Id}");
            }
            return await base.UpdateAsync(PackageTripDestination.Id, UpdateDto);
        }
        public override async Task<Result> DeleteAsync(int id)
        {
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetByIdAsync(id);
            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTripDestination With Id : {id}");

            }
            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                         .FirstOrDefaultAsync(x => x.PackageTripId == PackageTripDestination.PackageTripId);
            if (TripDate is not null)
            {
                return Result.BadRequest($" Cann't Delete PackageTrip Destination Because you have TripDate :{TripDate.Id} associated  with PackageTrip: {PackageTripDestination.PackageTripId}");

            }
            return await base.DeleteAsync(id);
        }

        public async Task<Result< GetPackageTripDestinationByIdDto>> GetPackageTripDestination(int packageTripId, int destinationId)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetByIdAsync(packageTripId);
            if (packageTrip is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found PackageTrip With id : {packageTripId}");
            }
            var Destination = await _destinationRepositoryAsync.GetByIdAsync(destinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found Destination With Id : {destinationId}");
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.PackageTripId == packageTripId && x.DestinationId == destinationId);
            if(PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationByIdDto>.NotFound($"Not Found packageTrip wiht id : {packageTripId} associated with Destination id {destinationId} ");
            }
            var resultDto = _mapper.Map<GetPackageTripDestinationByIdDto>(PackageTripDestination);
            return Result<GetPackageTripDestinationByIdDto>.Success(resultDto);
        }
    }
}
