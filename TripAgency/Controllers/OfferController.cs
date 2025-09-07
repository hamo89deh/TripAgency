using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PromotionDto;
using TripAgency.Service.Implementations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        public OfferController(IOfferService OfferService, IMapper mapper)
        {
            _offerService = OfferService;
            _mapper = mapper;
        }

        public IOfferService _offerService { get; }
        public IMapper _mapper { get; }


        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetOffersDto>>> GetAllAsync( )
        {
            var OffersResult = await _offerService.GetAllAsync();
            if (!OffersResult.IsSuccess)
                return this.ToApiResult(OffersResult);
            return ApiResult<IEnumerable<GetOffersDto>>.Ok(OffersResult.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetOfferByIdDto>> GetPromotionById(int Id)
        {
            var OffersResult = await _offerService.GetByIdAsync(Id);
            if (!OffersResult.IsSuccess)
                return this.ToApiResult(OffersResult);
            return ApiResult<GetOfferByIdDto>.Ok(OffersResult.Value!);
        }
   
        [HttpPost]
        public async Task<ApiResult<GetOfferByIdDto>> AddPromotion(AddOfferDto offerDto)
        {
            var OfferResult = await _offerService.CreateAsync(offerDto);
            if (!OfferResult.IsSuccess)
            {
                return this.ToApiResult(OfferResult);
            }
            return ApiResult<GetOfferByIdDto>.Created(OfferResult.Value!);
        }
      
        [HttpPut("{Id}")]
        public async Task<ApiResult<string>> UpdatOffer(int Id, UpdateOfferDto updateOffer)
        {
            var OfferResult = await _offerService.UpdateAsync(Id, updateOffer);
            if (!OfferResult.IsSuccess)
                return this.ToApiResult<string>(OfferResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeleteOffer(int Id)
        {
            var OfferResult = await _offerService.DeleteAsync(Id);
            if (!OfferResult.IsSuccess)
                return this.ToApiResult<string>(OfferResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
