using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Authontication;

namespace TripAgency.Service.Abstracts
{
    public interface IAuthonticationService
    {
        public Task<Result> ConfirmEmail(int userId, string code);
        public Task<Result<string>> SendConfirmEmailCode(string email);
        public Task<JwtAuthResult> GetJWTToken(User user);
       
    }
}
