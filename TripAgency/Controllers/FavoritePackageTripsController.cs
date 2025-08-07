using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.Favorite;
using TripAgency.Service.Implementations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritePackageTripsController : ControllerBase
    {
        public IFavoritePackageTripService _favoritePackageTripService {  get; set; }
        public FavoritePackageTripsController(IFavoritePackageTripService favoritePackageTripService)
        {
            _favoritePackageTripService = favoritePackageTripService;
        }
        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetFavoritePackageTripsDto>>> GetFavoritePackageTripsDto()
        {
            var favoritePackageTripsResult = await _favoritePackageTripService.GetFavoritePackageTripsDto();
            if (!favoritePackageTripsResult.IsSuccess)
                return this.ToApiResult(favoritePackageTripsResult);
            return ApiResult<IEnumerable<GetFavoritePackageTripsDto>>.Ok(favoritePackageTripsResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddFavoritePackageTripDto(int PackageTripId)
        {
            var addFavoritePackageTripResult = await _favoritePackageTripService.AddFavoritePackageTripDto(PackageTripId);
            if (!addFavoritePackageTripResult.IsSuccess)
                return this.ToApiResult<string>(addFavoritePackageTripResult);
            return ApiResult<string>.Ok(addFavoritePackageTripResult.Message!);
        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteFavoritePackageTripDto(int PackageTripId)
        {
            var DeleteFavoritePackageTripResult = await _favoritePackageTripService.DeleteFavoritePackageTripDto(PackageTripId);
            if (!DeleteFavoritePackageTripResult.IsSuccess)
                return this.ToApiResult<string>(DeleteFavoritePackageTripResult);
            return ApiResult<string>.Ok(DeleteFavoritePackageTripResult.Message!);
        }
    }
}
