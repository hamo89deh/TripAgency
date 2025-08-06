using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Abstracts
{
    public interface IAuthonticationService
    {
        public Task<Result> ConfirmEmail(int userId, string code);
        public Task<Result<string>> SendConfirmEmailCode(string email);
    }
}
