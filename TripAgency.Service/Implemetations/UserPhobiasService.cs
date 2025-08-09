using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.ActivityPhobia.Queries;
using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Service.Implementations
{
    public class UserPhobiasService : IUserPhobiaService
    {
        public IMapper _mapper { get; }
        public IPhobiaRepositoryAsync _phobiaRepositoryAsync { get; }
        public ICurrentUserService _currentUserService { get; }
        public IUserPhobiasRepositoryAsync _userPhobiasRepositoryAsync { get; }

        public UserPhobiasService(IActivityRepositoryAsync activityRepository,
                           IMapper mapper,
                           IPhobiaRepositoryAsync phobiaRepositoryAsync,
                           IActivityPhobiasRepositoryAsync activityPhobiasRepositoryAsync,
                           ICurrentUserService currentUserService,
                           IUserPhobiasRepositoryAsync userPhobiasRepositoryAsync ,
                            UserManager<User> userManager
                           )
        {
            _mapper = mapper;
            _phobiaRepositoryAsync = phobiaRepositoryAsync;
            _currentUserService = currentUserService;
            _userPhobiasRepositoryAsync = userPhobiasRepositoryAsync;
        }
        public async Task<Result> AddUserPhobias(AddUserPhobiasDto addUserPhobiasDto)
        {
            var User = await _currentUserService.GetUserAsync();

            var DistinctPhobiasId = new HashSet<int>();
            var DublicatePhobiaId = new HashSet<int>();
            foreach (var id in addUserPhobiasDto.Phobias)
                if (!DistinctPhobiasId.Add(id))
                    DublicatePhobiaId.Add(id);

            if (DublicatePhobiaId.Count() != 0)
            {
                return Result.BadRequest($"Duplicate Phobias Id : {string.Join(',', DublicatePhobiaId)}");
            }

            var existPhobias = await _phobiaRepositoryAsync.GetTableNoTracking()
                                                           .Where(a => DistinctPhobiasId.Contains(a.Id))
                                                           .ToListAsync();

            if (DistinctPhobiasId.Count() != existPhobias.Count())
            {
                var notFoundPhoniesId = DistinctPhobiasId.Except(existPhobias.Select(d => d.Id));
                return Result.NotFound($"One or More from Phonies Not found ,Missing Phonies Id : {string.Join(',', notFoundPhoniesId)} ");
            }
            var existUserPhobia = await _userPhobiasRepositoryAsync.GetTableNoTracking()
                                                                           .Where(x => x.UserId == User.Id)
                                                                           .ToListAsync();

            var PrePhobia = existUserPhobia.Where(x => DistinctPhobiasId.Contains(x.PhobiaId));
            if (PrePhobia.Count() != 0)
            {
                return Result.BadRequest($"the Phobia With Id : {string.Join(',', PrePhobia.Select(x => x.PhobiaId))} Adding Before");
            }
            foreach (var PhobiasId in DistinctPhobiasId)
            {
                await _userPhobiasRepositoryAsync.AddAsync(new UserPhobias
                {
                    UserId = User.Id,
                    PhobiaId = PhobiasId
                });
            };

            return Result.Success("Add Phobia To User");
        }

        public async Task<Result<GetUserPhobiasDto>> GetUserPhobiasAsync()
        {
            var user = await  _currentUserService.GetUserAsync();
            var userPhobias = await _userPhobiasRepositoryAsync.GetTableNoTracking()
                                                               .Where(x => x.UserId == user.Id)
                                                               .Include(x=>x.Phobia)
                                                               .ToListAsync();
            
            if (userPhobias.Count() == 0)
                return Result<GetUserPhobiasDto>.NotFound($"Not Found any Phobias for Uset With Id : {user.Id}");
            var result = new GetUserPhobiasDto()
            {
                UserId = user.Id,
                PhobiasDtos = new List<GetPhobiasDto>()
            };
            foreach (var userPhobia in userPhobias)
            {
                result.PhobiasDtos.Add(new GetPhobiasDto
                {
                    Id = userPhobia.PhobiaId,
                    Description = userPhobia.Phobia.Description,
                    Name = userPhobia.Phobia.Name
                });
            }
            return Result<GetUserPhobiasDto>.Success(result);
        }

        public async Task<Result> DeleteUserPhobia(int PhobiaId)
        {
            var user= await _currentUserService.GetUserAsync();
            var phobia = await _phobiaRepositoryAsync.GetTableNoTracking()
                                                     .FirstOrDefaultAsync(x => x.Id == PhobiaId);
            if (phobia is null)
            {
                return Result.NotFound($"Not Found Phobia With Id :{PhobiaId}");
            }
            var userPhobia = await _userPhobiasRepositoryAsync.GetTableNoTracking()
                                                              .FirstOrDefaultAsync(x => x.UserId == user.Id &&
                                                                                x.PhobiaId == PhobiaId)
                                                                      ;
            if (userPhobia is null)
            {
                return Result.NotFound($"Not Found User With Id :{user.Id} related With Phobia With Id :{PhobiaId}");
            }
            await _userPhobiasRepositoryAsync.DeleteAsync(userPhobia);
            return Result.Success("Deleted Success ");
        }
    }
}
