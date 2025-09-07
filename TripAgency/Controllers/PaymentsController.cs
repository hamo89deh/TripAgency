using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.Payment;
using TripAgency.Service.Implementations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        public IPaymentService _paymentService { get; }

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("SubmitManualPaymentNotification")]
        public async Task<ApiResult<string>> SubmitManualPaymentNotification(ManualPaymentDetailsDto ManualDto)
        {
            var SubmitManualPaymentResult = await _paymentService.SubmitManualPaymentNotificationAsync(ManualDto);
            if (!SubmitManualPaymentResult.IsSuccess)
            {
                return this.ToApiResult<string>(SubmitManualPaymentResult);
            }
            return ApiResult<string>.Created(SubmitManualPaymentResult.Message!);
        }
       
        [HttpPost("ProcessManualPaymentConfirmation")]
        public async Task<ApiResult<string>> ProcessManualPaymentConfirmation(ManualPaymentConfirmationRequestDto ManualDto)
        {
            var ProcessManualPaymentResult = await _paymentService.ProcessManualPaymentConfirmationAsync(ManualDto);
            if (!ProcessManualPaymentResult.IsSuccess)
            {
                return this.ToApiResult<string>(ProcessManualPaymentResult);
            }
            return ApiResult<string>.Created(ProcessManualPaymentResult.Message!);
        }
       
        [HttpPost("ReportMissingPayment")]
        public async Task<ApiResult<string>> ReportMissingPayment(MissingPaymentReportDto reportDto)
        {
            var ReportMissingPaymentResult = await _paymentService.ReportMissingPaymentAsync(reportDto);
            if (!ReportMissingPaymentResult.IsSuccess)
            {
                return this.ToApiResult<string>(ReportMissingPaymentResult);
            }
            return ApiResult<string>.Created(ReportMissingPaymentResult.Message!);
        }
      
        [HttpPost("ResolveMissingPaymentReport")]
        public async Task<ApiResult<string>> ResolveMissingPaymentReportAsync(DiscrepancyReportProcessRequestDto reportDto)
        {
            var ResolveMissingPaymentReportResult = await _paymentService.ResolveMissingPaymentReportAsync(reportDto);
            if (!ResolveMissingPaymentReportResult.IsSuccess)
            {
                return this.ToApiResult<string>(ResolveMissingPaymentReportResult);
            }
            return ApiResult<string>.Created(ResolveMissingPaymentReportResult.Message!);
        }
      
        [HttpGet("VerifyPaymentTransaction")]
        public async Task<ApiResult<PaymentTransactionStatusDto>> VerifyPaymentTransactionAsync(string TransactionRef)
        {
            var DetailsTransactionResult = await _paymentService.GetDetailsTransactionAsync(TransactionRef);
            if (!DetailsTransactionResult.IsSuccess)
            {
                return this.ToApiResult(DetailsTransactionResult);
            }
            return ApiResult<PaymentTransactionStatusDto>.Ok(DetailsTransactionResult.Value!);
        }
        [HttpGet("PaymentsPending")]
        public async Task<ApiResult<IEnumerable<ManualPaymentDetailsDto>>> GetPendingManualPaymentsForAdminAsync()
        {
            var PendingManualPayments = await _paymentService.GetPendingManualPaymentsForAdminAsync();
            if (!PendingManualPayments.IsSuccess)
            {
                return this.ToApiResult(PendingManualPayments);
            }
            return ApiResult<IEnumerable<ManualPaymentDetailsDto>>.Ok(PendingManualPayments.Value!);
        }
    
        [HttpGet("GetMissingPayment")]
        public async Task<ApiResult<IEnumerable<MissingPaymentReportResponceDto>>> GetMissingPaymentReportsForAdminAsync()
        {
            var MissingPaymentReportsResult = await _paymentService.GetMissingPaymentReportsForAdminAsync();
            if (!MissingPaymentReportsResult.IsSuccess)
            {
                return this.ToApiResult(MissingPaymentReportsResult);
            }
            return ApiResult<IEnumerable<MissingPaymentReportResponceDto>>.Ok(MissingPaymentReportsResult.Value!);
        }

        [HttpGet("paymentMethods")]
        public async Task<ApiResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethod()
        {
            var PaymentMethodDtoResult = await _paymentService.GetPaymentMethods();
            if (!PaymentMethodDtoResult.IsSuccess)
            {
                return this.ToApiResult(PaymentMethodDtoResult);
            }
            return ApiResult<IEnumerable<PaymentMethodDto>>.Ok(PaymentMethodDtoResult.Value!);
        }
    }
}
