using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
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
        public async Task<ApiResult<string>> UpdateStatudPackageTripDate(UpdatePackageTripDateDto tripDate)
        {
            var packageTripDateResult = await _packageTripDateService.UpdateStatusTripDate(tripDate);
            if (!packageTripDateResult.IsSuccess)
            {
                return this.ToApiResult<string>(packageTripDateResult);
            }
            return ApiResult<string>.Ok(packageTripDateResult.Message);
        }

    }

}
