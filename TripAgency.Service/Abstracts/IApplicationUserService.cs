using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.User.Command;
using TripAgency.Service.Feature.User.Queries;
using TripAgency.Service.Implemetations;

namespace TripAgency.Service.Abstracts
{
    public interface IApplicationUserService
    {
        public Task<Result> AddUser(AddUserDto userDtos);
        public Task<Result> UpdateUser(UpdateUserDto userDtos);
        public Task<Result> DeleteUser(int Id);
        public Task<Result<string>> ChangePassword(ChangeUserPassword changeUserPassword);
        public Task<Result<GetUserByIdDto>> GetUserById(int Id);
        public Task<Result<IEnumerable<GetUsersDto>>> GetUsers();

    }
}
