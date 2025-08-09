using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Refund.Commmand;
using TripAgency.Service.Feature.Refund.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundsController : ControllerBase
    {
        public IRefundService _refundService { get; }
        public RefundsController(IRefundService refundService)
        {
            _refundService = refundService;
        }
        [HttpGet("RefundsPending")]
        public async Task<ApiResult<IEnumerable<GetRefundsPendingDto>>> GetRefundsPending()
        {
            var ResultRefundsPendingDto = await _refundService.GetRefundsPendingDto();
            if(!ResultRefundsPendingDto.IsSuccess)
            {
                return this.ToApiResult(ResultRefundsPendingDto);
            }
            return ApiResult<IEnumerable<GetRefundsPendingDto>>.Ok(ResultRefundsPendingDto.Value!);
        }
        [HttpGet("RefundsCompleted")]
        public async Task<ApiResult<IEnumerable<GetRefundsCompletedDto>>> GetRefundsCompleted()
        {
            var ResultRefundsCompletedDto = await _refundService.GetRefundsCompletedDto();
            if (!ResultRefundsCompletedDto.IsSuccess)
            {
                return this.ToApiResult(ResultRefundsCompletedDto);
            }
            return ApiResult<IEnumerable<GetRefundsCompletedDto>>.Ok(ResultRefundsCompletedDto.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetRefundByIdDto>> GetRefundById(int Id)
        {
            var ResultRefundByIdDto = await _refundService.GetRefundByIdAsync(Id);
            if (!ResultRefundByIdDto.IsSuccess)
            {
                return this.ToApiResult(ResultRefundByIdDto);
            }
            return ApiResult<GetRefundByIdDto>.Ok(ResultRefundByIdDto.Value!);
        }

        [HttpPost]
        public async Task<ApiResult<string>> ConfirmRefund(ConfirmRefundDto confirmRefundDto)
        {
            var ResultConfirmRefund = await _refundService.ConfirmRefund(confirmRefundDto);
            if (!ResultConfirmRefund.IsSuccess)
            {
                return this.ToApiResult<string>(ResultConfirmRefund);
            }
            return ApiResult<string>.Ok(ResultConfirmRefund.Message!);
        }

    }
}
