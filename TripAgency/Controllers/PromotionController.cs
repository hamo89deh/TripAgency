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
    public class PromotionController : ControllerBase
    {
        public PromotionController(IPromotionService PromotionService, IMapper mapper)
        {
            _promotionService = PromotionService;
            _mapper = mapper;
        }

        public IPromotionService _promotionService { get; }
        public IMapper _mapper { get; }

        [HttpGet("PackageTrip/{PackageTripId}")]
        public async Task<ApiResult<IEnumerable<GetPromotionsDto>>> GetPromotionsByPackageTripIdAsync(int PackageTripId)
        {
            var PromotionsResult = await _promotionService.GetPromotionsByPackageTripIdAsync(PackageTripId);
            if (!PromotionsResult.IsSuccess)
                return this.ToApiResult(PromotionsResult);
            return ApiResult<IEnumerable<GetPromotionsDto>>.Ok(PromotionsResult.Value!);
        }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetPromotionsDto>>> GetAllAsync( )
        {
            var PromotionsResult = await _promotionService.GetAllAsync();
            if (!PromotionsResult.IsSuccess)
                return this.ToApiResult(PromotionsResult);
            return ApiResult<IEnumerable<GetPromotionsDto>>.Ok(PromotionsResult.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetPromotionByIdDto>> GetPromotionById(int Id)
        {
            var PromotionResult = await _promotionService.GetByIdAsync(Id);
            if (!PromotionResult.IsSuccess)
                return this.ToApiResult(PromotionResult);
            return ApiResult<GetPromotionByIdDto>.Ok(PromotionResult.Value!);
        }
        [HttpGet("Valid/PackageTrip{PackagTripId}")]
        public async Task<ApiResult<GetPromotionByIdDto>> GetValidPromotionAsync(int PackagTripId)
        {
            var PromotionResult = await _promotionService.GetValidPromotionAsync(PackagTripId);
            if (!PromotionResult.IsSuccess)
                return this.ToApiResult(PromotionResult);
            return ApiResult<GetPromotionByIdDto>.Ok(PromotionResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<GetPromotionByIdDto>> AddPromotion(AddPromotionDto Promotion)
        {
            var PromotionResult = await _promotionService.CreateAsync(Promotion);
            if (!PromotionResult.IsSuccess)
            {
                return this.ToApiResult(PromotionResult);
            }
            return ApiResult<GetPromotionByIdDto>.Created(PromotionResult.Value!);
        }
      
        [HttpPut("{Id}")]
        public async Task<ApiResult<string>> UpdatePromotion(int Id, UpdatePromotionDto updatePromotion)
        {
            var PromotionResult = await _promotionService.UpdateAsync(Id, updatePromotion);
            if (!PromotionResult.IsSuccess)
                return this.ToApiResult<string>(PromotionResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeletePromotion(int Id)
        {
            var PromotionResult = await _promotionService.DeleteAsync(Id);
            if (!PromotionResult.IsSuccess)
                return this.ToApiResult<string>(PromotionResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
