using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripType.Commands;
using TripAgency.Service.Feature.PackageTripType.Queries;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;

namespace TripAgency.Service.Implemetations
{
    public class PackageTripTypesService : IPackageTripTypesService
    {
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; set; }
        public IPackageTripTypeRepositoryAsync _packageTripTypeRepositoryAsync { get; }
        public ITripTypeRepositoryAsync _tripTypeRepositoryAsync { get; }
        public PackageTripTypesService(ITripTypeRepositoryAsync tripTypeRepositoryAsync,
                                       IPackageTripRepositoryAsync packageTripRepositoryAsync,
                                       IPackageTripTypeRepositoryAsync packageTripTypeRepositoryAsync)
        {
            _tripTypeRepositoryAsync = tripTypeRepositoryAsync;
            _packageTripRepositoryAsync = packageTripRepositoryAsync;
            _packageTripTypeRepositoryAsync = packageTripTypeRepositoryAsync;
        }


        public async Task<Result> AddPackageTripTypes(AddPackageTripTypesDto addPackageTripTypesDto)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetByIdAsync(addPackageTripTypesDto.PackageTripId);
            if (packageTrip is null)
                return Result.NotFound($"Not Found PackageTrip with Id : {addPackageTripTypesDto.PackageTripId}");


            var DistinctTripTypeId = new HashSet<int>();
            var DublicateTripTypeId = new HashSet<int>();
            foreach (var id in addPackageTripTypesDto.TripTypes)
                if (!DistinctTripTypeId.Add(id))
                    DublicateTripTypeId.Add(id);

            if (DublicateTripTypeId.Count() != 0)
            {
                return Result.BadRequest($"Duplicate TripType Id : {string.Join(',', DublicateTripTypeId)}");
            }

            var existTripType = await _tripTypeRepositoryAsync.GetTableNoTracking()
                                                           .Where(a => DistinctTripTypeId.Contains(a.Id))
                                                           .ToListAsync();

            if (DistinctTripTypeId.Count() != existTripType.Count())
            {
                var notFoundTripTypesId = DistinctTripTypeId.Except(existTripType.Select(d => d.Id));
                return Result.NotFound($"One or More from Trip Type Not found ,Missing Trip Type Id : {string.Join(',', notFoundTripTypesId)} ");
            }
            var existPackageTripType = await _packageTripTypeRepositoryAsync.GetTableNoTracking()
                                                         .Where(x => x.PackageTripId == packageTrip.Id)
                                                         .ToListAsync();

            var PreTripType = existPackageTripType.Where(x => DistinctTripTypeId.Contains(x.TypeTripId));
            if (PreTripType.Count() != 0)
            {
                return Result.BadRequest($"the TripType With Id : {string.Join(',', PreTripType.Select(x => x.TypeTripId))} Adding Before");
            }
            foreach (var TripTypeId in DistinctTripTypeId)
            {
                await _packageTripTypeRepositoryAsync.AddAsync(new PackageTripType
                {
                    TypeTripId = TripTypeId,
                    PackageTripId = packageTrip.Id
                });
            };

            return Result.Success("Add TripType To PackageTrip Success");
        }

        public async Task<Result> DeletePackageTripType(int PackageTripId, int TripTypeId)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                  .FirstOrDefaultAsync(x => x.Id == PackageTripId);
            if (packageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id :{PackageTripId}");
            }
            var TripType = await _tripTypeRepositoryAsync.GetTableNoTracking()
                                                     .FirstOrDefaultAsync(x => x.Id == TripTypeId);
            if (TripType is null)
            {
                return Result.NotFound($"Not Found TripType With Id :{TripTypeId}");
            }
            var PackageTripType = await _packageTripTypeRepositoryAsync.GetTableNoTracking()
                                                                      .FirstOrDefaultAsync(x => x.PackageTripId == PackageTripId &&
                                                                                x.TypeTripId == TripTypeId)
                                                                      ;
            if (PackageTripType is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id :{PackageTripId} related With TripType With Id :{TripTypeId}");
            }
            await _packageTripTypeRepositoryAsync.DeleteAsync(PackageTripType);
            return Result.Success("Deleted Success ");
        }

        public async Task<Result<GetPackageTripTypesDto>> GetPackageTripTypesAsync(int PackageTripId)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                         .Where(x => x.Id == PackageTripId)
                                                         .Include(x => x.PackageTripTypes)
                                                         .ThenInclude(x => x.TypeTrip)
                                                         .FirstOrDefaultAsync();
            if (PackageTrip is null)
            {
                return Result<GetPackageTripTypesDto>.NotFound($"Not Found any PackageTrip With id : {PackageTripId}");

            }
            if (PackageTrip.PackageTripTypes.Count() == 0)
                return Result<GetPackageTripTypesDto>.NotFound($"Not Found any TypeTrip for PackageTrip With Id : {PackageTripId}");
            var result = new GetPackageTripTypesDto()
            {
                PackageTripId = PackageTripId,
                TripTypesDtos = new List<GetTripTypesDto>()
            };
            foreach (var packageTripType in PackageTrip.PackageTripTypes)
            {
                result.TripTypesDtos.Add(new GetTripTypesDto
                {
                    Id = packageTripType.TypeTripId,
                    Name = packageTripType.TypeTrip.Name

                });
            }
            return Result<GetPackageTripTypesDto>.Success(result);
        }
    }
}
