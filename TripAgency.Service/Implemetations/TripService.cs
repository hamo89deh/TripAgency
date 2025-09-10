using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using TripAgency.Data.Entities;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Enums;
using TripAgency.Data.Helping;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Repositories;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.OfferDto;
using TripAgency.Service.Feature.Trip.Commands;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Generic;
using TripAgency.Service.Implemetations;

namespace TripAgency.Service.Implementations
{
    public class TripService : GenericService<Trip, GetTripByIdDto, GetTripsDto, AddTripDto, UpdateTripDto>, ITripService
    {
        private ITripRepositoryAsync _tripRepository { get; set; }
        private IDestinationRepositoryAsync _destinationRepositoryAsync { get; set; }
        public ICurrentUserService _currentUserService { get; }
        public IMediaService _mediaService { get; }
        public IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; }
        public IUserPhobiasRepositoryAsync _userPhobiaRepositoryAsync { get; }
        public ITripReviewService _tripReviewService { get; }
        public ITripReviewRepositoryAsync _tripReviewRepository { get; }
        private ITripDestinationRepositoryAsync _tripDestinationRepositoryAsync { get; set; }
        private readonly IOffersRepositoryAsync _promotionRepositoryAsync;
        public IMapper _mapper { get; }

        public TripService(ITripRepositoryAsync tripRepository,
                           IMapper mapper,
                           IDestinationRepositoryAsync destinationRepositoryAsync,
                           ICurrentUserService currentUserService,
                           IMediaService mediaService,
                           IPackageTripRepositoryAsync packageTripRepositoryAsync,
                           IUserPhobiasRepositoryAsync userPhobiaRepositoryAsync,
                           IOffersRepositoryAsync promotionRepository,
                           ITripReviewService tripReviewService,
                           ITripDestinationRepositoryAsync tripDestinationRepositoryAsync) : base(tripRepository, mapper)
        {
            _tripRepository = tripRepository;
            _mapper = mapper;
            _destinationRepositoryAsync = destinationRepositoryAsync;
            _currentUserService = currentUserService;
            _mediaService = mediaService;
            _packageTripRepositoryAsync = packageTripRepositoryAsync;
            _userPhobiaRepositoryAsync = userPhobiaRepositoryAsync;
            _tripDestinationRepositoryAsync = tripDestinationRepositoryAsync;
            _promotionRepositoryAsync = promotionRepository;
            _tripReviewService = tripReviewService;
        }
        public async Task<Result<GetTripByIdDto>> GetTripByNameAsync(string name)
        {
            var trip = await _tripRepository.GetTripByName(name);
            if (trip is null)
                return Result<GetTripByIdDto>.NotFound($"Not Found Trip With Name : {name}");
            var tripResult = _mapper.Map<GetTripByIdDto>(trip);
            return Result<GetTripByIdDto>.Success(tripResult);

        }

        public async Task<Result<GetTripDestinationsDto>> AddTripDestinations(AddTripDestinationsDto addTripDestinationsDto)
        {
            var trip = await _tripRepository.GetTableNoTracking()
                                            .Where(t => t.Id == addTripDestinationsDto.TripId)
                                            .Include(td => td.TripDestinations)
                                            .FirstOrDefaultAsync();
            if (trip is null)
                return Result<GetTripDestinationsDto>.NotFound($"Not Found Trip With Id : {addTripDestinationsDto.TripId}");

            var requestedDestinationIds = addTripDestinationsDto.DestinationIdDto.Select(d => d.DestinationId)
                                                                                 .Distinct()
                                                                                 .ToList();

            if (requestedDestinationIds.Count != addTripDestinationsDto.DestinationIdDto.Count())
            {
                return Result<GetTripDestinationsDto>.BadRequest("Duplicate destination IDs found in the request.");
            }

            var existingDestinations = await _destinationRepositoryAsync.GetTableNoTracking()
                                                                        .Where(d => requestedDestinationIds.Contains(d.Id))
                                                                        .ToListAsync();

            if (existingDestinations.Count() != requestedDestinationIds.Count())
            {
                var notFoundDestinationIds = requestedDestinationIds.Except(existingDestinations.Select(d => d.Id)).ToList();

                return Result<GetTripDestinationsDto>.NotFound($"One or more destinations not found. Missing Destination Ids: {string.Join(", ", notFoundDestinationIds)}");
            }

            var tripDestinationsToAdd = new List<TripDestination>();
            foreach (var destination in existingDestinations)
            {
                // Check if the trip already has this destination to avoid duplicates
                if (!trip.TripDestinations.Any(td => td.DestinationId == destination.Id))
                {
                    tripDestinationsToAdd.Add(new TripDestination
                    {
                        TripId = trip.Id,
                        DestinationId = destination.Id
                    });
                }
            }

            if (tripDestinationsToAdd.Any())
            {
                await _tripDestinationRepositoryAsync.AddRangeAsync(tripDestinationsToAdd);
            }

            // Map 
            var resultDto = new GetTripDestinationsDto
            {
                TripId = trip.Id,
                DestinationsDto = existingDestinations.Select(d => new GetDestinationByIdDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CityId = d.CityId,
                    Description = d.Description,
                    Location = d.Location
                }).ToList()
            };

            return Result<GetTripDestinationsDto>.Success(resultDto);
        }

        public async Task<Result<GetTripDestinationsDto>> GetTripDestinationsById(int TripId)
        {
            var trip = await _tripRepository.GetByIdAsync(TripId);
            if (trip is null)
                return Result<GetTripDestinationsDto>.NotFound($"Not Found Trip With Id : {TripId}");

            var tripDestinations = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                        .Where(td => td.TripId == TripId)
                                                                        .Include(d => d.Destination)
                                                                        .ToListAsync();

            var resultDto = new GetTripDestinationsDto
            {
                TripId = trip.Id,
                DestinationsDto = tripDestinations.Select(d => new GetDestinationByIdDto
                {
                    Id = d.Destination.Id,
                    Description = d.Destination.Description,
                    CityId = d.Destination.CityId,
                    Location = d.Destination.Location,
                    Name = d.Destination.Name,
                    ImageUrl = d.Destination.ImageUrl,
                })

            };
            return Result<GetTripDestinationsDto>.Success(resultDto);
        }

        public async Task<Result> DeleteTripDestination(int tripId, int destinationId)
        {
            var trip = await _tripRepository.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == tripId);
            if (trip is null)
                return Result.NotFound($"Not Found Trip With Id : {tripId}");

            var destination = await _destinationRepositoryAsync.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == destinationId);
            if (destination is null)
                return Result.NotFound($"Not Found Destination With Id : {destinationId}");

            var tripDestination = await _tripDestinationRepositoryAsync.GetTableNoTracking()
                                                                        .FirstOrDefaultAsync(td => td.TripId == trip.Id && td.DestinationId == destination.Id);
            if (tripDestination is null)
                return Result.NotFound($"Not Found Trip Destination relationship between Trip Id: {tripId} and Destination Id: {destinationId}");

            await _tripDestinationRepositoryAsync.DeleteAsync(tripDestination);
            return Result.Success("Deleted successfully");


        }
        public override async Task<Result<GetTripByIdDto>> CreateAsync(AddTripDto AddDto)
        {
            var mapTrip = _mapper.Map<Trip>(AddDto);
            mapTrip.ImageUrl = await _mediaService.UploadMediaAsync("Trip", AddDto.Image);
            await _tripRepository.AddAsync(mapTrip);

            var resultTrip = _mapper.Map<GetTripByIdDto>(mapTrip);

            return Result<GetTripByIdDto>.Success(resultTrip);
        }
        public override async Task<Result> UpdateAsync(int id, UpdateTripDto UpdateDto)

        {
            var trip = await _tripRepository.GetTableNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (trip is null)
                return Result.NotFound($"Not Found Trip with Id : {id}");

            trip.Name = UpdateDto.Name;
            trip.Description = UpdateDto.Description;
            var oldImageUrl = "";
            if (UpdateDto.Image is not null)
            {
                oldImageUrl = trip.ImageUrl;
                trip.ImageUrl = await _mediaService.UploadMediaAsync("Trip", UpdateDto.Image);
            }
            await _tripRepository.UpdateAsync(trip);

            if (!string.IsNullOrEmpty(oldImageUrl))
                await _mediaService.DeleteMediaAsync(oldImageUrl);
            return Result.Success("update Successfully");

        }
        public override async Task<Result<IEnumerable<GetTripsDto>>> GetAllAsync()
        {
            var trips = await _tripRepository.GetTableNoTracking()
                                             .Include(p => p.PackageTrips)
                                             .ToListAsync();
            var tripResult = new List<GetTripsDto>();
            foreach (var trip in trips)
            {
                tripResult.Add(new GetTripsDto
                {
                    Id = trip.Id,
                    Name = trip.Name,
                    Description = trip.Description,
                    ImageUrl = trip.ImageUrl,
                    CountPackageTrip = trip.PackageTrips.Count()
                });
            }
            return Result<IEnumerable<GetTripsDto>>.Success(tripResult);
        }
        public async Task<Result<IEnumerable<GetTripsDto>>> GetTripsForUsersAsync()
        {
            
            User user = null! ;
            var userPhobiaIds = new List<int>();
            try
            {
                 user = await _currentUserService.GetUserAsync();
                 userPhobiaIds = await _userPhobiaRepositoryAsync.GetTableNoTracking()
                                                             .Where(p => p.UserId == user.Id)
                                                             .Select(x => x.PhobiaId)
                                                             .ToListAsync();
            }
            catch (UnauthorizedAccessException)
            {
            }


            // جلب الرحلات التي تحتوي على PackageTripDates بحالة Published فقط
            // جلب الرحلات التي تحتوي على PackageTrips مع PackageTripDates بحالة Published
            var tripsQuery = _tripRepository.GetTableNoTracking()
                                            .Include(t => t.PackageTrips)
                                                .ThenInclude(pt => pt.PackageTripDates)
                                            .Include(t => t.PackageTrips)
                                                .ThenInclude(pt => pt.PackageTripDestinations)
                                                    .ThenInclude(ptd => ptd.PackageTripDestinationActivities)
                                                        .ThenInclude(ptda => ptda.Activity)
                                                            .ThenInclude(a => a.ActivityPhobias)
                                            .Where(t => t.PackageTrips.Any(pt => pt.PackageTripDates.Any(ptd => ptd.Status == PackageTripDateStatus.Published && DateTime.Now.Date >= ptd.StartBookingDate.Date)));

            // تصفية الرحلات بناءً على فوبيا المستخدم إذا كان موجودًا
            var trips = await tripsQuery
                .Select(t => new
                {
                    Trip = t,
                    ValidPackageTrips = t.PackageTrips
                        .Where(pt => pt.PackageTripDates.Any(ptd => ptd.Status == PackageTripDateStatus.Published) &&
                                     (user == null ||
                                      pt.PackageTripDestinations.All(ptd =>
                                          ptd.PackageTripDestinationActivities.All(ptda =>
                                              !ptda.Activity.ActivityPhobias.Any(ap => userPhobiaIds.Contains(ap.PhobiaId)))))
                        )
                        .ToList()
                })
                .Where(t => t.ValidPackageTrips.Any())
                .Select(t => new GetTripsDto
                {
                    Id = t.Trip.Id,
                    Name = t.Trip.Name,
                    Description = t.Trip.Description,
                    ImageUrl = t.Trip.ImageUrl,
                    CountPackageTrip = t.ValidPackageTrips.Count
                })
                .ToListAsync();
            if (trips.Count() == 0)
                return Result<IEnumerable<GetTripsDto>>.NotFound("Not Found Any Trips");
            return Result<IEnumerable<GetTripsDto>>.Success(trips);
        }
        public async Task<Result<GetPackageTripsForTripDto>> GetPackagesForTripAsync(int TripId)
        {
            User user = null!;
            var userPhobiaIds = new List<int>();
            try
            {
                user = await _currentUserService.GetUserAsync();
                userPhobiaIds = await _userPhobiaRepositoryAsync.GetTableNoTracking()
                                                           .Where(p => p.UserId == user.Id)
                                                           .Select(p => p.PhobiaId)
                                                           .Distinct()
                                                           .ToListAsync();

            }
            catch (UnauthorizedAccessException)
            {
            }


            var trip = await _tripRepository.GetTableNoTracking()
                                                 .Where(x => x.Id == TripId)
                                                 .FirstOrDefaultAsync();
            if (trip is null)
                return Result<GetPackageTripsForTripDto>.NotFound($"Not Found Trip By Id : {TripId}");

            var PackageTrips = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                          .Include(x => x.PackageTripDates)
                                                          .Include(x => x.PackageTripDestinations)
                                                             .ThenInclude(x => x.PackageTripDestinationActivities)
                                                                .ThenInclude(x => x.Activity)
                                                                    .ThenInclude(x=>x.ActivityPhobias)
                                                          .Where(x => x.TripId == trip.Id && x.PackageTripDates.Any(x => x.Status == PackageTripDateStatus.Published && DateTime.Now.Date >= x.StartBookingDate.Date ) &&
                                                                 (user == null ||
                                                                  x.PackageTripDestinations.All(ptd =>
                                                                      ptd.PackageTripDestinationActivities.All(ptda =>
                                                                          !ptda.Activity.ActivityPhobias.Any(ap => userPhobiaIds.Contains(ap.PhobiaId)))))
                                                                )
                                                          .Include(x=>x.PackageTripOffers)
                                                          .ThenInclude(x=>x.Offer)
                                                          .Include(x => x.PackageTripDestinations)
                                                             .ThenInclude(x => x.Destination)
                                                                .ThenInclude(x => x.City)                                                        
                                                          .Include(x => x.PackageTripTypes)
                                                             .ThenInclude(x => x.TypeTrip)

                                                          .ToListAsync();
            if (!PackageTrips.Any())
                return Result<GetPackageTripsForTripDto>.NotFound($"Not Found Any PackageTrip Published for Trip With Id: {trip.Id}");
            
            var packageTripForTripDtos = new List<PackageTripForTripDto>();

            if (PackageTrips.Select(x => x.PackageTripDates).Count() == 0)
                return Result<GetPackageTripsForTripDto>.NotFound($"Not Found Any PackageTrip Publish for Trip With Id:{trip.Id}");
            foreach (var packageTrip in PackageTrips)
            {
                // جلب المدن الفريدة بناءً على City.Id
                var cities = packageTrip.PackageTripDestinations
                                       .Select(pd => new PackageTripCitiesDto
                                       {
                                           Id = pd.Destination.City.Id,
                                           Name = pd.Destination.City.Name
                                       })
                                       .GroupBy(c => c.Id)
                                       .Select(g => g.First())
                                       .ToList();

               
                // حساب السعر الأصلي
                var actualPrice = packageTrip.Price + packageTrip.PackageTripDestinations.Sum(ptd => ptd.PackageTripDestinationActivities.Sum(ptda => ptda.Price));

             
                // التحقق من العرض الترويجي
                var offer = packageTrip.PackageTripOffers.Where(x => x.IsApply).Select(p => p.Offer).FirstOrDefault(x => x.IsActive && x.EndDate >= DateTime.Now && x.StartDate <= DateTime.Now);
                // حساب السعر بعد الخصم
                decimal? priceAfterOffer = null;
                GetOfferByIdDto OfferDto = null;
                if (offer != null )
                {
                    priceAfterOffer = actualPrice * (1 - (offer.DiscountPercentage / 100m));
                    OfferDto = _mapper.Map<GetOfferByIdDto>(offer);
                }
                // حساب متوسط التقييم
                int finalRating = await _tripReviewService.CalculateAverageRatingAsync(packageTrip.Id);
                packageTripForTripDtos.Add(new PackageTripForTripDto
                {
                    PackageTripId = packageTrip.Id,
                    ActulPrice = packageTrip!.Price + packageTrip!.PackageTripDestinations.Sum(ptd => ptd.PackageTripDestinationActivities.Sum(ptda => ptda.Price)),
                    PriceAfterOffer = priceAfterOffer,
                    GetOfferByIdDto = OfferDto,
                    TripId = packageTrip.TripId,
                    Name = packageTrip.Name,
                    Description = packageTrip.Description,
                    ImageUrl = packageTrip.ImageUrl,
                    Duration = packageTrip.Duration,
                    MaxCapacity = packageTrip.MaxCapacity,
                    MinCapacity = packageTrip.MinCapacity,
                    PackageTripCitiyDto = cities,
                    Rating= finalRating,
                    PackageTripTypesDtos = packageTrip.PackageTripTypes.Select(x => new PackageTripTypesForTripDto
                    {
                        Id = x.TypeTripId,
                        Name = x.TypeTrip.Name
                    })


                }); ;
            }
            var result = new GetPackageTripsForTripDto()
            {
                TripId = TripId,
                PackageTripForTripDtos = packageTripForTripDtos
            };

            return Result<GetPackageTripsForTripDto>.Success(result);
        }
        public async Task<Result<PaginatedResult<GetTripsDto>>> GetTripsPaginationForUsersAsync(string? search, int pageNumber, int pageSize)
        { 
            
            User user = null! ;
            var userPhobiaIds = new List<int>();
            try
            {
                 user = await _currentUserService.GetUserAsync();
                 userPhobiaIds = await _userPhobiaRepositoryAsync.GetTableNoTracking()
                                                            .Where(p => p.UserId == user.Id)
                                                            .Select(p=>p.PhobiaId)
                                                            .Distinct()
                                                            .ToListAsync();
                                                             
            }
            catch (UnauthorizedAccessException)
            {
            }


            // جلب الرحلات التي تحتوي على PackageTripDates بحالة Published فقط
            // جلب الرحلات التي تحتوي على PackageTrips مع PackageTripDates بحالة Published
            var tripsQuery = _tripRepository.GetTableNoTracking()
                                            .Include(t => t.PackageTrips)
                                                .ThenInclude(pt => pt.PackageTripDates)
                                            .Where(t => t.PackageTrips.Any(pt => pt.PackageTripDates.Any(ptd => ptd.Status == PackageTripDateStatus.Published && DateTime.Now.Date >= ptd.StartBookingDate.Date)))
                                            .Include(t => t.PackageTrips)
                                                .ThenInclude(pt => pt.PackageTripDestinations)
                                                    .ThenInclude(ptd => ptd.PackageTripDestinationActivities)
                                                        .ThenInclude(ptda => ptda.Activity)
                                                            .ThenInclude(a => a.ActivityPhobias);

            // تصفية الرحلات بناءً على فوبيا المستخدم إذا كان موجودًا
            var tripsResult = await tripsQuery
                .Select(t => new
                {
                    Trip = t,
                    ValidPackageTrips = t.PackageTrips
                        .Where(pt => pt.PackageTripDates.Any(ptd => ptd.Status == PackageTripDateStatus.Published) &&
                                     (user == null ||
                                      pt.PackageTripDestinations.All(ptd =>
                                          ptd.PackageTripDestinationActivities.All(ptda =>
                                              !ptda.Activity.ActivityPhobias.Any(ap => userPhobiaIds.Contains(ap.PhobiaId)))))
                        )
                        .ToList()
                })
                .Where(t => t.ValidPackageTrips.Any())
                .Select(t => new GetTripsDto
                {
                    Id = t.Trip.Id,
                    Name = t.Trip.Name,
                    Description = t.Trip.Description,
                    ImageUrl = t.Trip.ImageUrl,
                    CountPackageTrip = t.ValidPackageTrips.Count
                })
                .ToPaginatedListAsync(pageNumber, pageSize);
            if (tripsResult.TotalCount == 0)
                return Result<PaginatedResult<GetTripsDto>>.NotFound("Not found Any Trips");
            return Result<PaginatedResult<GetTripsDto>>.Success(tripsResult);
        }      
        public async Task<Result<PaginatedResult<PackageTripForTripDto>>> GetPackagesPaginationForTripAsync(int TripId, string? search, int pageNumber, int pageSize)
        {
            User user = null!;
            var userPhobiaIds = new List<int>();
            try
            {
                user = await _currentUserService.GetUserAsync();
                userPhobiaIds = await _userPhobiaRepositoryAsync.GetTableNoTracking()
                                                           .Where(p => p.UserId == user.Id)
                                                           .Select(p => p.PhobiaId)
                                                           .ToListAsync();

            }
            catch (UnauthorizedAccessException)
            {
            }

            // التحقق من وجود الرحلة
            var trip = await _tripRepository.GetTableNoTracking()
                                           .Where(x => x.Id == TripId)
                                           .FirstOrDefaultAsync();
            if (trip == null)
                return Result<PaginatedResult<PackageTripForTripDto>>.NotFound($"Not Found Trip By Id : {TripId}");

            // بناء الاستعلام الأساسي
            var query = _packageTripRepositoryAsync.GetTableNoTracking();
               
            // تطبيق البحث إذا وجد
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.ApplySearch(search , new[] {"Name", "Description"});
            }
            query = query.Include(x => x.PackageTripDates)
                         .Include(x => x.PackageTripDestinations)
                            .ThenInclude(x => x.PackageTripDestinationActivities)
                               .ThenInclude(x => x.Activity)
                                  .ThenInclude(x => x.ActivityPhobias)
                .Where(x => x.TripId == trip.Id && x.PackageTripDates.Any(x => x.Status == PackageTripDateStatus.Published && DateTime.Now.Date >= x.StartBookingDate.Date) &&
                                     (user == null ||
                                      x.PackageTripDestinations.All(ptd =>
                                          ptd.PackageTripDestinationActivities.All(ptda =>
                                              !ptda.Activity.ActivityPhobias.Any(ap => userPhobiaIds.Contains(ap.PhobiaId))))))
                .Include(x => x.PackageTripOffers)
                    .ThenInclude(x=>x.Offer)
                .Include(x => x.PackageTripDestinations)
                    .ThenInclude(x => x.Destination)
                        .ThenInclude(x => x.City)            
                .Include(x => x.PackageTripTypes)
                    .ThenInclude(x => x.TypeTrip);

            // بناء DTO داخل الاستعلام مع تطبيق Pagination
            var paginatedResult = await query.Select(pt => new PackageTripForTripDto
            {
                PackageTripId = pt.Id,
                ActulPrice = pt.Price + pt.PackageTripDestinations.Sum(ptd => ptd.PackageTripDestinationActivities.Sum(ptda => ptda.Price)),
                PriceAfterOffer = pt.PackageTripOffers.Any(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now)
                ? pt.Price + pt.PackageTripDestinations.Sum(ptd => ptd.PackageTripDestinationActivities.Sum(ptda => ptda.Price)) *
                  (1 - (pt.PackageTripOffers.First(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now).Offer.DiscountPercentage / 100m))
                : null,
                GetOfferByIdDto = pt.PackageTripOffers.Any(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now)
                ? new GetOfferByIdDto
                {
                    Id = pt.PackageTripOffers.First(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now).Offer.Id,
                    DiscountPercentage = pt.PackageTripOffers.First(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now).Offer.DiscountPercentage,
                    StartDate = pt.PackageTripOffers.First(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now).Offer.StartDate,
                    EndDate = pt.PackageTripOffers.First(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateTime.Now && x.Offer.StartDate <= DateTime.Now).Offer.EndDate
                }
                : null,
                TripId = pt.TripId ,
                Name = pt.Name,
                Description = pt.Description,
                ImageUrl = pt.ImageUrl,
                Duration = pt.Duration,
                MaxCapacity = pt.MaxCapacity,
                MinCapacity = pt.MinCapacity,
                PackageTripCitiyDto = pt.PackageTripDestinations
                    .Select(pd => new PackageTripCitiesDto
                    {
                        Id = pd.Destination.City.Id,
                        Name = pd.Destination.City.Name
                    })
                    .GroupBy(c => c.Id)
                    .Select(g => g.First()),
                Rating = pt.Rate, // استخدام العمود الجديد Rate
                PackageTripTypesDtos = pt.PackageTripTypes.Select(x => new PackageTripTypesForTripDto
                {
                    Id = x.TypeTripId,
                    Name = x.TypeTrip.Name
                })
            }).ToPaginatedListAsync(pageNumber, pageSize);


            if (paginatedResult.TotalCount == 0)
                return Result<PaginatedResult<PackageTripForTripDto>>.NotFound($"Not Found Any PackageTrip Published for Trip With Id: {trip.Id}");

            return Result<PaginatedResult<PackageTripForTripDto>>.Success(paginatedResult);
        }
        public override async Task<Result> DeleteAsync(int id)
        {
            var trip = await _tripRepository.GetTableNoTracking()
                                                 .Where(x => x.Id == id)
                                                 .Include(x=>x.PackageTrips)
                                                 .Include(x=>x.TripDestinations)
                                                 .FirstOrDefaultAsync();

            if (trip is null)
                return Result.NotFound($"Not Found Trip By Id : {id}");

            if (trip.PackageTrips.Any())
                return Result.BadRequest($"Cannot delete Trip with associated PackageTrips.");

            if (trip.TripDestinations.Any())
                return Result.BadRequest($"Cannot delete Trip with associated Trip Destinations.");

            return await base.DeleteAsync(id);        
                
        }
    }
}
