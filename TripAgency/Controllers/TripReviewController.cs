using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripReview;
using TripAgency.Service.Implementations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripReviewController : ControllerBase
    {
        public TripReviewController(ITripReviewService TripReviewService, IMapper mapper)
        {
            _tripreviewService = TripReviewService;
            _mapper = mapper;
        }

        public ITripReviewService _tripreviewService { get; }
        public IMapper _mapper { get; }


        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetTripReviewsDto>>> GetAllAsync()
        {
            var TripReviewsResult = await _tripreviewService.GetAllAsync();
            if (!TripReviewsResult.IsSuccess)
                return this.ToApiResult(TripReviewsResult);
            return ApiResult<IEnumerable<GetTripReviewsDto>>.Ok(TripReviewsResult.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetTripReviewByIdDto>> GetTripReviewById(int Id)
        {
            var TripReviewResult = await _tripreviewService.GetByIdAsync(Id);
            if (!TripReviewResult.IsSuccess)
                return this.ToApiResult(TripReviewResult);
            return ApiResult<GetTripReviewByIdDto>.Ok(TripReviewResult.Value!);
        }
     
        [HttpPost]
        public async Task<ApiResult<GetTripReviewByIdDto>> AddTripReview(AddTripReviewDto TripReview)
        {
            var TripReviewResult = await _tripreviewService.CreateAsync(TripReview);
            if (!TripReviewResult.IsSuccess)
            {
                return this.ToApiResult(TripReviewResult);
            }
            return ApiResult<GetTripReviewByIdDto>.Created(TripReviewResult.Value!);
        }

        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeleteTripReview(int Id)
        {
            var TripReviewResult = await _tripreviewService.DeleteAsync(Id);
            if (!TripReviewResult.IsSuccess)
                return this.ToApiResult<string>(TripReviewResult);
            return ApiResult<string>.Ok("Success Delete");
        }

        [HttpGet("can-review/{packageTripDateId}")]
        public async Task<ApiResult<bool>> CanUserReview(int packageTripDateId)
        {
            var result = await _tripreviewService.CanUserReviewAsync(packageTripDateId);
            if (!result.IsSuccess)
                return this.ToApiResult(result);
            return ApiResult<bool>.Ok(result.Value);
        }
    }
}
