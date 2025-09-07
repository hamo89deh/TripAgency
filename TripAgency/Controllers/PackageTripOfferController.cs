using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTripOfferController : ControllerBase
    {
        public PackageTripOfferController(IPackageTripOffersService PackageTripOfferService, IMapper mapper)
        {
            _packageTripOfferService = PackageTripOfferService;
            _mapper = mapper;
        }

        public IPackageTripOffersService _packageTripOfferService { get; }
        public IMapper _mapper { get; }

        [HttpPost]
        public async Task<ApiResult<string>> AddPackageTripOffer(int packageTripId, int offerId)
        {
            var OffersResult = await _packageTripOfferService.AddPackageTripOffer(packageTripId, offerId);
            if (!OffersResult.IsSuccess)
                return this.ToApiResult<string>(OffersResult);
            return ApiResult<string>.Ok(OffersResult.Message);
        }

        [HttpDelete("PackageTrip/{packageTripId}/Offer/{offerId}")]
        public async Task<ApiResult<string>> DeletePackageTripOffer(int packageTripId, int offerId)
        {
            var OffersResult = await _packageTripOfferService.DeletePackageTripOffer(packageTripId, offerId);
            if (!OffersResult.IsSuccess)
                return this.ToApiResult<string>(OffersResult);
            return ApiResult<string>.Ok(OffersResult.Message!);
        }
        [HttpPut("Cancelling/{packageTripId}")]
        public async Task<ApiResult<string>> CancelAppliedOfferAsync(int packageTripId)
        {
            var OffersResult = await _packageTripOfferService.CancelAppliedOfferAsync(packageTripId);
            if (!OffersResult.IsSuccess)
                return this.ToApiResult<string>(OffersResult);
            return ApiResult<string>.Ok(OffersResult.Message!);
        }
        [HttpGet("PackageTrip/{packageTripId}")]
        public async Task<ApiResult<IEnumerable<GetPackageTripOffersDto>>> GetOffersByPackageTripIdAsync(int packageTripId)
        {
            var OffersResult = await _packageTripOfferService.GetOffersByPackageTripIdAsync(packageTripId);
            if (!OffersResult.IsSuccess)
                return this.ToApiResult(OffersResult);
            return ApiResult<IEnumerable<GetPackageTripOffersDto>>.Ok(OffersResult.Value!);
        }
        [HttpGet("Valid/PackageTrip{PackagTripId}")]
        public async Task<ApiResult<GetOfferByIdDto>> GetValidOffersAsync(int PackagTripId)
        {
            var OfferResult = await _packageTripOfferService.GetValidOfferAsync(PackagTripId);
            if (!OfferResult.IsSuccess)
                return this.ToApiResult(OfferResult);
            return ApiResult<GetOfferByIdDto>.Ok(OfferResult.Value!);
        }
    }
}
