using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Helping;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Favorite;

namespace TripAgency.Service.Implementations
{
    public class FavoritePackageTripService : IFavoritePackageTripService
    {
        private readonly IPackageTripDateRepositoryAsync _packageTripDateRepository;

        public IFavoritePackageTripRepositoryAsync _favoritePackageTripRepository { get; set; }
        public UserManager<User> _userManager { get; set; }
        public IHttpContextAccessor _httpContextAccessor { get; }
        public ICurrentUserService _currentUserService { get; }

        public FavoritePackageTripService(IFavoritePackageTripRepositoryAsync favoritePackageTripRepository,
                                          UserManager<User> userManager ,
                                          IHttpContextAccessor httpContextAccessor,
                                          IPackageTripDateRepositoryAsync packageTripDateRepository,
                                          ICurrentUserService currentUserService
                                          )
        {
            _favoritePackageTripRepository = favoritePackageTripRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _packageTripDateRepository = packageTripDateRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result> AddFavoritePackageTripDto(int PackageTripId)
        {
            var userId =  _currentUserService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Result.NotFound($"Not Found User by Id :{userId}");
            var packageTrip = await _packageTripDateRepository.GetTableNoTracking()
                                                               .FirstOrDefaultAsync(p => p.Id == PackageTripId);
            if(packageTrip is null)
            {
                return Result.NotFound($"PackageTrip With Id : {PackageTripId}");
            }
            await _favoritePackageTripRepository.AddAsync(new FavoritePackageTrip
            {
                PackageTripId = PackageTripId,
                UserId = user.Id
            });
            return Result.Success("Adding To Favorite Successing");

        }
        public async Task<Result> DeleteFavoritePackageTripDto(int PackageTripId)
        {
            var userId = _currentUserService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Result.NotFound($"Not Found User by Id :{userId}");
            var packageTrip = await _packageTripDateRepository.GetTableNoTracking()
                                                               .FirstOrDefaultAsync(p => p.Id == PackageTripId);
            if (packageTrip is null)
            {
                return Result.NotFound($"PackageTrip With Id : {PackageTripId}");
            }
            var favoritePackageTrip = await _favoritePackageTripRepository.GetTableNoTracking()
                                                                          .FirstOrDefaultAsync(f => f.PackageTripId == PackageTripId &&
                                                                                                  f.UserId == user.Id);
            if (favoritePackageTrip is null)
                return Result.NotFound($"Not Found PacageTrip With id : {packageTrip.Id} in Favorite User : {user.Id}");

            await _favoritePackageTripRepository.DeleteAsync(favoritePackageTrip);
            return Result.Success("Deleting From Favorite Successing");
        }
        public async Task<Result<IEnumerable<GetFavoritePackageTripsDto>>> GetFavoritePackageTripsDto()
        {
            var userId = _currentUserService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Result<IEnumerable<GetFavoritePackageTripsDto >>.NotFound($"Not Found User by Id :{userId}");

            var favoritePackageTripsDto = await _favoritePackageTripRepository.GetTableNoTracking()
                                                                              .Where(f => f.UserId == user.Id)
                                                                              .Include(f=>f.PackageTrip)
                                                                              .ToListAsync();

            if (favoritePackageTripsDto.Count() == 0)
            {
                return Result<IEnumerable<GetFavoritePackageTripsDto>>.NotFound("Not Found Any package in favorite");
            }
            var GetFavoritePackageTripsDto = new List<GetFavoritePackageTripsDto>();
           
            foreach (var favoritePackageTrip in favoritePackageTripsDto)
            {
                GetFavoritePackageTripsDto.Add(new GetFavoritePackageTripsDto
                {
                    Description = favoritePackageTrip.PackageTrip.Description,
                    Name = favoritePackageTrip.PackageTrip.Name,
                    PackageTripId = favoritePackageTrip.PackageTripId
                });
            }
            return Result<IEnumerable<GetFavoritePackageTripsDto>>.Success( GetFavoritePackageTripsDto);

        }
    }
}
