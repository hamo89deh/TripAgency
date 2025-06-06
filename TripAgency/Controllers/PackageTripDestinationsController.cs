using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;

namespace TripAgency.Api.Controllers
{
    public class PackageTripDestinationsController : ControllerBase
    {
        public PackageTripDestinationsController(IPackageTripDestinationService packageTripDestinationService, IMapper mapper)
        {
            _packageTripDestinationService = packageTripDestinationService;
            _mapper = mapper;
        }

        public IPackageTripDestinationService _packageTripDestinationService { get; }
        public IMapper _mapper { get; }
        public async Task<ApiResult<GetPackageTripDestinationByIdDto>> AddPackageTripDestination(AddPackageTripDestinationDto packageTripDestination)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.CreateAsync(packageTripDestination);
            if (!packageTripDestinationResult.IsSuccess)
            {
                return this.ToApiResult(packageTripDestinationResult);
            }
            return ApiResult<GetPackageTripDestinationByIdDto>.Created(packageTripDestinationResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdatePackageTripDestination(UpdatePackageTripDestinationDto updatePackageTripDestination)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.UpdateAsync(updatePackageTripDestination.Id, updatePackageTripDestination);
            if (!packageTripDestinationResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeletePackageTripDestination(int id)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.DeleteAsync(id);
            if (!packageTripDestinationResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
