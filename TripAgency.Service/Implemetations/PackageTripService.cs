using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripService : GenericService<PackageTrip, GetPackageTripByIdDto, GetPackageTripsDto, AddPackageTripDto, UpdatePackageTripDto>, IPackageTripService
    {
        private IPackageTripRepositoryAsync _packagetripRepository { get; set; }
        public ITripRepositoryAsync _tripRepositoryAsync { get; }
        public IMapper _mapper { get; }

        public PackageTripService(IPackageTripRepositoryAsync packagetripRepository, 
                                  ITripRepositoryAsync tripRepository,
                                  IMapper mapper
                           ) : base(packagetripRepository, mapper)
        {
            _packagetripRepository = packagetripRepository;
            _tripRepositoryAsync = tripRepository;
            _mapper = mapper;
        }

        public override async Task<Result<GetPackageTripByIdDto>> CreateAsync(AddPackageTripDto AddDto)
        {
            var trip = await _tripRepositoryAsync.GetByIdAsync(AddDto.TripId);
            if (trip is null)
                return Result<GetPackageTripByIdDto>.NotFound($"Not Found Trip By Id : {AddDto.TripId}");
            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDto UpdateDto)
        {
            var trip = await _tripRepositoryAsync.GetByIdAsync(UpdateDto.TripId);
            if (trip is null)
                return Result.NotFound($"Not Found Trip By Id : {UpdateDto.TripId}");
            return await base.UpdateAsync(id, UpdateDto);
        }
    }
}
