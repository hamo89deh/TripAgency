using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.NewFolder1;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Repositories;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.PackageTripType.Commands;
using TripAgency.Service.Feature.Payment;
using TripAgency.Service.Feature.PromotionDto;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripService : GenericService<PackageTrip, GetPackageTripByIdDto, GetPackageTripsDto, AddPackageTripDto, UpdatePackageTripDto>, IPackageTripService
    {
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; set; }
        public IPackageTripDateRepositoryAsync _packagetripDateRepository { get; }
        public ILogger<PackageTripService> _logger { get; }
        public ICityRepositoryAsync _cityRepositoryAsync { get; }
        public IOffersRepositoryAsync _promotionRepository { get; }
        public ITripRepositoryAsync _tripRepositoryAsync { get; }
        public ITripReviewService _tripReviewService { get; }
        public IMediaService _mediaService { get; }
        public IMapper _mapper { get; }

        public PackageTripService(IPackageTripRepositoryAsync packagetripRepository,
                                  IPackageTripDateRepositoryAsync packagetripDateRepository,
                                  ICityRepositoryAsync cityRepositoryAsync,
                                  IOffersRepositoryAsync promotionRepository,
                                  ITripRepositoryAsync tripRepository,
                                  ITripReviewService tripReviewService,
                                  ILogger<PackageTripService> logger,
                                  IMediaService mediaService,
                                  IMapper mapper
                                  ) : base(packagetripRepository, mapper)
        {
            _packageTripRepositoryAsync = packagetripRepository;
            _packagetripDateRepository = packagetripDateRepository;
            _cityRepositoryAsync = cityRepositoryAsync;
            _promotionRepository = promotionRepository;
            _tripRepositoryAsync = tripRepository;
            _tripReviewService = tripReviewService;
            _mediaService = mediaService;
            _mapper = mapper;
            _logger= logger;
        }

        public override async Task<Result<GetPackageTripByIdDto>> CreateAsync(AddPackageTripDto AddDto)
        {
            var trip = await _tripRepositoryAsync.GetByIdAsync(AddDto.TripId);
            if (trip is null)
                return Result<GetPackageTripByIdDto>.NotFound($"Not Found Trip By Id : {AddDto.TripId}");
            var mapPackageTrip = _mapper.Map<PackageTrip>(AddDto);
            mapPackageTrip.ImageUrl = await _mediaService.UploadMediaAsync("PackageTrip", AddDto.Image);
            await _packageTripRepositoryAsync.AddAsync(mapPackageTrip);
            var result = new GetPackageTripByIdDto()
            {
                Id = mapPackageTrip.Id,
                Duration = AddDto.Duration,
                ImageUrl = mapPackageTrip.ImageUrl,
                MaxCapacity = AddDto.MaxCapacity,
                Description = AddDto.Description,
                MinCapacity = AddDto.MinCapacity,
                Name = AddDto.Name,
                Price = AddDto.Price,
                TripId = AddDto.TripId
            }
            ;
            return Result<GetPackageTripByIdDto>.Success(result);
        }

        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDto UpdateDto)
        {
            var trip = await _tripRepositoryAsync.GetByIdAsync(UpdateDto.TripId);
            if (trip is null)
                return Result.NotFound($"Not Found Trip By Id : {UpdateDto.TripId}");
            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                               .Where(x => x.Id == id)
                                                               .Include(x => x.PackageTripDates)
                                                               .Select(x => new PackageTrip
                                                               {
                                                                   Description = x.Description,
                                                                   Duration = x.Duration,
                                                                   ImageUrl = x.ImageUrl,
                                                                   Id = x.Id,
                                                                   MaxCapacity = x.MaxCapacity,
                                                                   MinCapacity = x.MinCapacity,
                                                                   Name = x.Name,
                                                                   Price = x.Price,
                                                                   TripId = x.TripId,
                                                                   PackageTripDates = x.PackageTripDates.Where(x => x.Status != PackageTripDateStatus.Completed && x.Status != PackageTripDateStatus.Cancelled),
                                                               })
                                                               .FirstOrDefaultAsync();
            if (packageTrip is null)
                return Result.NotFound($"Not Found PackageTrip By Id : {id}");
            using var transaction = await _packagetripDateRepository.BeginTransactionAsync();
            try
            {
                var PackageTripDateNotCanUpdateCapacity = new List<PackageTripDate>();
                if (packageTrip.PackageTripDates.Count() != 0)
                {
                    foreach (var PackageTripDate in packageTrip.PackageTripDates)
                    {
                        if (UpdateDto.MaxCapacity >= packageTrip.MaxCapacity - PackageTripDate.AvailableSeats)
                        {
                            PackageTripDate.AvailableSeats += UpdateDto.MaxCapacity - packageTrip.MaxCapacity;
                            if (PackageTripDate.AvailableSeats == 0 && PackageTripDate.Status != PackageTripDateStatus.BookingClosed)
                                PackageTripDate.Status = PackageTripDateStatus.Full;
                            await _packagetripDateRepository.UpdateAsync(PackageTripDate);
                            continue;
                        }
                        PackageTripDateNotCanUpdateCapacity.Add(PackageTripDate);

                    }
                }
                if (PackageTripDateNotCanUpdateCapacity.Count() != 0)
                {
                    await transaction.RollbackAsync();
                    return Result.BadRequest($"Dates with ids: {string.Join(",", PackageTripDateNotCanUpdateCapacity.Select(x => x.Id))} for PackagaTrip  Not have space to update");
                }
                packageTrip.Duration = UpdateDto.Duration;
                packageTrip.Description = UpdateDto.Description;
                packageTrip.TripId = UpdateDto.TripId;
                packageTrip.Name = UpdateDto.Name;
                packageTrip.MaxCapacity = UpdateDto.MaxCapacity;
                packageTrip.MinCapacity = UpdateDto.MinCapacity;
                var oldImageUrl = "";
                if (UpdateDto.Image is not null)
                {
                    oldImageUrl= packageTrip.ImageUrl;
                    packageTrip.ImageUrl = await _mediaService.UploadMediaAsync("PackageTrip", UpdateDto.Image);
                }
                await _packageTripRepositoryAsync.UpdateAsync(packageTrip);
                await transaction.CommitAsync();
                if (!string.IsNullOrEmpty(oldImageUrl))
                    await _mediaService.DeleteMediaAsync(oldImageUrl);
                return Result.Success("Update Successfully");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Result<GetPackageTripDestinationsActivitiesDatesDto>> GetPackageTripDestinationsActivitiesDates(int packageTripId, enPackageTripDataStatusDto status)
        {
            var PackageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                             .Where(x => x.Id == packageTripId)
                                                             .Include(x => x.PackageTripDestinations)
                                                             .ThenInclude(x => x.PackageTripDestinationActivities)
                                                             .ThenInclude(x=>x.Activity)
                                                             .Include(x => x.PackageTripDates.Where(ptd => ptd.Status == Global.ConvertEnPackageTripDataStatusDtoToPackageTripDataStatus(status)))
                                                             .FirstOrDefaultAsync();

            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found PackageTrip With Id : {packageTripId}");
            }



            if (!PackageTrip.PackageTripDestinations.Any())
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found any Destination for PackageTrip : {packageTripId}");
            }

            if (!PackageTrip.PackageTripDestinations.Select(d => d.PackageTripDestinationActivities).Any())
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found any Activities for Destination {string.Join(",", PackageTrip.PackageTripDestinations.Select(d => d.Id))} to PackageTrip : {packageTripId}");
            }

            if (!PackageTrip.PackageTripDates.Any())
            {
                return Result<GetPackageTripDestinationsActivitiesDatesDto>.NotFound($"Not Found any Dates for PackageTrip : {packageTripId}");
            }
            var resultDto = new GetPackageTripDestinationsActivitiesDatesDto()
            {
                PackageTripId = packageTripId,
                DestinationsActivitiesDtos = PackageTrip.PackageTripDestinations.Select(d => new PackageTripDestinationsActivitiesDto
                {
                    //DayNumber = d.DayNumber,
                    //Description = d.Description,
                    //Duration = d.Duration,
                    //EndTime = d.EndTime,
                    //StartTime = d.StartTime,
                    //OrderDestination = d.OrderDestination,
                    DestinationId = d.DestinationId,
                    ActivitiesDtos = d.PackageTripDestinationActivities.Select(a => new GetPackageTripDestinationActivityDto
                    {
                        //Description = a.Description,
                        //Duration = a.Duration,
                        //EndTime = a.EndTime,
                        //OrderActivity = a.OrderActivity,
                        //StartTime = d.StartTime,
                        Name = a.Activity.Name,
                        Price = a.Price,
                        ActivityId = a.ActivityId,
                    })
                }),
                PackageTripDatesDtos = PackageTrip.PackageTripDates.Where(ptd => ptd.Status == Global.ConvertEnPackageTripDataStatusDtoToPackageTripDataStatus(status)).Select(d => new PackageTripDatesDto
                {
                    Id = d.Id,
                    EndBookingDate = d.EndBookingDate,
                    StartBookingDate = d.StartBookingDate,
                    StartPackageTripDate = d.StartPackageTripDate,
                    EndPackageTripDate = d.EndPackageTripDate,
                    Status = (enPackageTripDataStatusDto)d.Status

                })
            };
            return Result<GetPackageTripDestinationsActivitiesDatesDto>.Success(resultDto);
        }

        public async Task<Result<GetPackageTripDetailsDto>> GetPackageTripDetailsAsync(int packageTripId)
        {
            _logger.LogInformation("Fetching PackageTrip details for PackageTripId: {PackageTripId}", packageTripId);

            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                .Include(x => x.PackageTripDates)
                
                .Include(x => x.PackageTripOffers)
                .ThenInclude(x=>x.Offer)
                .Include(x => x.PackageTripDestinations)
                    .ThenInclude(x => x.Destination)
                        .ThenInclude(x => x.City)
                            .ThenInclude(x => x.Hotels)
                .Include(x => x.PackageTripDestinations)
                    .ThenInclude(x => x.PackageTripDestinationActivities)
                        .ThenInclude(x => x.Activity)
                .Include(x => x.PackageTripTypes)
                    .ThenInclude(x => x.TypeTrip)
                .Where(x => x.Id == packageTripId)
                .FirstOrDefaultAsync();

            if (packageTrip == null)
            {
                _logger.LogWarning("PackageTrip with Id {PackageTripId} not found.", packageTripId);
                return Result<GetPackageTripDetailsDto>.NotFound($"PackageTrip with Id {packageTripId} not found.");
            }

            // حساب السعر الأصلي
            var actualPrice = packageTrip.Price + packageTrip.PackageTripDestinations.Sum(ptd => ptd.PackageTripDestinationActivities.Sum(ptda => ptda.Price));

            // التحقق من العرض الترويجي
            var offer = packageTrip.PackageTripOffers
                                        .Where(x => x.IsApply)
                                        .Select(p => p.Offer)
                                        .FirstOrDefault(x => x.IsActive
                                                        && x.EndDate >= DateOnly.FromDateTime(DateTime.Now)
                                                        && x.StartDate <= DateOnly.FromDateTime(DateTime.Now)
                                                        );
            if (offer != null && offer.IsActive && offer.EndDate < DateOnly.FromDateTime(DateTime.Now))
            {
                // تعطيل العرض المنتهي
                offer.IsActive = false;
                await _promotionRepository.UpdateAsync(offer);
                _logger.LogInformation("Deactivated expired Offer {OfferId} for PackageTrip {PackageTripId}", offer.Id, packageTrip.Id);
                offer = null;
            }

            // حساب السعر بعد الخصم
            decimal? priceAfterOffer = null;
            GetOfferByIdDto offerDto = null;
            if (offer != null && offer.IsActive && offer.StartDate <= DateOnly.FromDateTime(DateTime.Now) && offer.EndDate >= DateOnly.FromDateTime(DateTime.Now))
            {
                priceAfterOffer = actualPrice * (1 - (offer.DiscountPercentage / 100m));
                offerDto = _mapper.Map<GetOfferByIdDto>(offer);
            }

            // جلب المدن الفريدة
            var cities = packageTrip.PackageTripDestinations
                .Select(pd => new PackageTripCitiesDto
                {
                    Id = pd.Destination.City.Id,
                    Name = pd.Destination.City.Name
                })
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .ToList();

            // جلب الفنادق الفريدة
            var hotels = packageTrip.PackageTripDestinations
                .SelectMany(pd => pd.Destination.City.Hotels)
                .Select(h => new PackageTripHotelsDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    CityId=h.CityId,
                    CityName = h.City.Name,
                    Email = h.Email,
                    Location= h.Location,  
                    MidPriceForOneNight = h.MidPriceForOneNight,
                    Phone = h.Phone,
                    Rate = h.Rate
                   
                    
                })
                .GroupBy(h => h.Id)
                .Select(g => g.First())
                .ToList();
            // حساب متوسط التقييم
            int finalRating = await _tripReviewService.CalculateAverageRatingAsync(packageTripId);
            // إنشاء DTO
            var resultDto = new GetPackageTripDetailsDto
            {
                PackageTripId = packageTrip.Id,
                Name = packageTrip.Name,
                Description = packageTrip.Description,
                Duration = packageTrip.Duration,
                MaxCapacity = packageTrip.MaxCapacity,
                MinCapacity = packageTrip.MinCapacity,
                Rating = finalRating,
                ActualPrice = actualPrice,
                PriceAfterPromotion = priceAfterOffer,
                GetOfferByIdDto = offerDto,
                ImageUrl = packageTrip.ImageUrl,
                TripId = packageTrip.TripId,
                PackageTripCitiesDto = cities,
                PackageTripHotelsDto = hotels,
                PackageTripTypesDtos = packageTrip.PackageTripTypes.Select(x => new PackageTripTypesForTripDto
                {
                    Id = x.TypeTripId,
                    Name = x.TypeTrip.Name
                }),
                PackageTripDestinationsDtos = packageTrip.PackageTripDestinations.Select(x => new PackageTripDestinationsForTripDto
                {
                    Id = x.DestinationId,
                    Name = x.Destination.Name,
                    packageTripDestinationActivitiesForTrips = x.PackageTripDestinationActivities.Select(a => new PackageTripDestinationActivitiesForTripDto
                    {
                        Id = a.ActivityId,
                        Name = a.Activity.Name
                    })
                }),
                PackageTripDates = packageTrip.PackageTripDates
                    .Where(ptd => ptd.Status == PackageTripDateStatus.Published)
                    .Select(ptd => new PackageTripDatesForTripDto
                    {
                        Id = ptd.Id,
                        StartPackageTripDate = ptd.StartPackageTripDate,
                        EndPackageTripDate = ptd.EndPackageTripDate,
                        StartBookingDate = ptd.StartBookingDate,
                        EndBookingDate = ptd.EndBookingDate,
                        AvailableSeats = ptd.AvailableSeats
                    })
            };

            _logger.LogInformation("Successfully fetched PackageTrip details for PackageTripId: {PackageTripId}", packageTripId);
            return Result<GetPackageTripDetailsDto>.Success(resultDto);
        }
    }
}
