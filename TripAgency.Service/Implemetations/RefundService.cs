using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Refund.Commmand;
using TripAgency.Service.Feature.Refund.Queries;

namespace TripAgency.Service.Implemetations
{
    public class RefundService : IRefundService
    {
        public IRefundRepositoryAsync _refundRepositoryAsync { get; }
        public IPaymentRepositoryAsync _paymentRepositoryAsync { get; }

        public RefundService(IRefundRepositoryAsync refundRepositoryAsync ,
                             IPaymentRepositoryAsync paymentRepositoryAsync)
        {
            _refundRepositoryAsync = refundRepositoryAsync;
            _paymentRepositoryAsync = paymentRepositoryAsync;
        }
        public  async Task<Result> ConfirmRefund(ConfirmRefundDto confirmRefundDto)
        {
            var refund = await _refundRepositoryAsync.GetTableNoTracking()
                                                     .Where(x=>x.Id==confirmRefundDto.Id)
                                                     .Include(x=>x.Payment)
                                                     .FirstOrDefaultAsync();
            if (refund is null)
                return Result.NotFound($"Not Found Refund by id : {confirmRefundDto.Id}");
           
            refund.RefundProcessedDate = DateTime.Now;
            refund.Status = RefundStatus.Completed;
            refund.TransactionRefunded = confirmRefundDto.TransactionRefunded;
            refund.AdminNotes = confirmRefundDto.AdminNotes;

            await _refundRepositoryAsync.UpdateAsync(refund);

            if (refund.Payment is not null)
            {
                refund.Payment.PaymentStatus = PaymentStatus.Refunded;
                await _paymentRepositoryAsync.UpdateAsync(refund.Payment);
            }
            
            return Result.Success("Confirm Refund Success");
        }

        public async Task<Result<GetRefundByIdDto>> GetRefundByIdAsync(int Id)
        {
            var refund = await _refundRepositoryAsync.GetTableNoTracking()
                                                     .Where(x => x.Id == Id)
                                                     .FirstOrDefaultAsync();
            if (refund is null)
                return Result<GetRefundByIdDto>.NotFound($"Not Found Refund by id : {Id}");
            
            var result = new GetRefundByIdDto
            {
                Id = Id,
                Amount = refund.Amount,
                Status = refund.Status,
                TransactionReference = refund.TransactionReference,
                CreatedAt = refund.CreatedAt,
                RefundProcessedDate = refund.UpdatedAt,
                PaymentId = refund.PaymentId,
                ReportId = refund.ReportId,
                TransactionRefunded = refund.TransactionRefunded,
                AdminNotes = refund.AdminNotes,

            };
           
            return Result<GetRefundByIdDto>.Success(result);
        }

        public async Task<Result<IEnumerable<GetRefundsCompletedDto>>> GetRefundsCompletedDto()
        {
            var refundsCompleted= await _refundRepositoryAsync.GetTableNoTracking()
                                                   .Where(x => x.Status == RefundStatus.Completed)
                                                   .ToListAsync();
            if (refundsCompleted.Count()==0)
                return Result<IEnumerable<GetRefundsCompletedDto>>.NotFound($"Not Found Refunds Completed");
            var result = new List<GetRefundsCompletedDto>();
            foreach (var refund in refundsCompleted)
            {
                result.Add(
                new GetRefundsCompletedDto
                {
                    Id = refund.Id,
                    Amount = refund.Amount,
                    Status = refund.Status,
                    TransactionReference = refund.TransactionReference,
                    CreatedAt = refund.CreatedAt,
                    RefundProcessedDate = refund.UpdatedAt,
                    PaymentId = refund.PaymentId,
                    ReportId = refund.ReportId,
                    TransactionRefunded = refund.TransactionRefunded,
                    AdminNotes = refund.AdminNotes,
                });
            }
            return Result<IEnumerable<GetRefundsCompletedDto>>.Success(result);
        }

        public async Task<Result<IEnumerable<GetRefundsPendingDto>>> GetRefundsPendingDto()
        {
            var refundsPending = await _refundRepositoryAsync.GetTableNoTracking()
                                                     .Where(x => x.Status == RefundStatus.Pending)
                                                     .ToListAsync();
            if (refundsPending.Count() == 0)
                return Result<IEnumerable<GetRefundsPendingDto>>.NotFound($"Not Found Refunds Pending");
            var result = new List<GetRefundsPendingDto>();
            foreach (var refund in refundsPending)
            {
                result.Add(
                new GetRefundsPendingDto
                {
                    Id = refund.Id,
                    Amount = refund.Amount,
                    Status = refund.Status,
                    TransactionReference = refund.TransactionReference,
                    CreatedAt = refund.CreatedAt,
                    PaymentId = refund.PaymentId,
                    ReportId = refund.ReportId,                  
                });
            }
            return Result<IEnumerable<GetRefundsPendingDto>>.Success(result);
        }
    }
}
