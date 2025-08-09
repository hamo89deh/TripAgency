using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.Refund.Commmand;
using TripAgency.Service.Feature.Refund.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IRefundService 
    {
        Task<Result<GetRefundByIdDto>> GetRefundByIdAsync(int Id);
        Task<Result<IEnumerable<GetRefundsPendingDto>>> GetRefundsPendingDto();
        Task<Result<IEnumerable<GetRefundsCompletedDto>>> GetRefundsCompletedDto();
        Task<Result> ConfirmRefund(ConfirmRefundDto confirmRefundDto);
    }
}
