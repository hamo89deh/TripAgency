using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Enums;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTripDatesController : ControllerBase
    {
        public PackageTripDatesController(IPackageTripDateService packageTripDateService, IMapper mapper)
        {
            _packageTripDateService = packageTripDateService;
            _mapper = mapper;
        }

        public IPackageTripDateService _packageTripDateService { get; }
        public IMapper _mapper { get; }

        
      
        [HttpPost]
        public async Task<ApiResult<GetPackageTripDateByIdDto>> AddPackageTripDate(AddPackageTripDateDto tripDate)
        {

            var packageTripDateResult = await _packageTripDateService.CreateAsync(tripDate);
            if (!packageTripDateResult.IsSuccess)
            {
                return this.ToApiResult(packageTripDateResult);
            }
            return ApiResult<GetPackageTripDateByIdDto>.Created(packageTripDateResult.Value!);
        }

        [HttpPut]
        public async Task<ApiResult<string>> UpdateStatudPackageTripDate(int DatePackageTripId , enUpdatePackageTripDataStatusDto Status)
        {
            var packageTripDateResult = await _packageTripDateService.UpdateStatusTripDate(new UpdatePackageTripDateDto
            {
                Id = DatePackageTripId,
                Status= Status
            });
            if (!packageTripDateResult.IsSuccess)
            {
                return this.ToApiResult<string>(packageTripDateResult);
            }
            return ApiResult<string>.Ok(packageTripDateResult.Message);
        }
      
        [HttpGet("PackageTrip/{PackageTripId}")]
        public async Task<ApiResult<IEnumerable<GetPackageTripDateByIdDto>>> DatePackageTrip(int PackageTripId, PackageTripDateStatus? dateStatus )
        {
            var packageTripDateResult = await _packageTripDateService.GetDateForPackageTrip(PackageTripId, dateStatus);
            if (!packageTripDateResult.IsSuccess)
            {
                return this.ToApiResult<IEnumerable<GetPackageTripDateByIdDto>> (packageTripDateResult);
            }
            return ApiResult<IEnumerable<GetPackageTripDateByIdDto>>.Ok(packageTripDateResult.Value!);
        }

    }

}
