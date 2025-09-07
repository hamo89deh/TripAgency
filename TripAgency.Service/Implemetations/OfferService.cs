using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PromotionDto;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class OfferService : GenericService<Offer, GetOfferByIdDto, GetOffersDto, AddOfferDto, UpdateOfferDto>, IOfferService
    {
        private readonly IOffersRepositoryAsync _promotionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OfferService> _logger;

        public IPackageTripOffersRepositoryAsync _promotionPackageTripsRepo { get; }
        public IBookingTripRepositoryAsync _bookingTripRepository { get; }
        public IPackageTripRepositoryAsync _packageTripRepository { get; }

        public OfferService(IOffersRepositoryAsync offerRepository,
                                IPackageTripOffersRepositoryAsync promotionPackageTripsRepo,
                                IBookingTripRepositoryAsync bookingTripRepository,
                                IPackageTripRepositoryAsync packageTripRepository,
                                IMapper mapper,
                                ILogger<OfferService> logger
            ) : base(offerRepository, mapper)
        {
            _promotionRepository = offerRepository;
            _promotionPackageTripsRepo = promotionPackageTripsRepo;
            _bookingTripRepository = bookingTripRepository;
            _packageTripRepository = packageTripRepository;
            _mapper = mapper;
            _logger = logger;
        }



        // إضافة عرض جديد
        public override async Task<Result<GetOfferByIdDto>> CreateAsync(AddOfferDto dto)
        {
            // التحقق من صحة التواريخ
            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Now))
                return Result<GetOfferByIdDto>.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate < dto.StartDate)
                return Result<GetOfferByIdDto>.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result<GetOfferByIdDto>.BadRequest("DiscountPercentage must be between 1 and 100.");
            
            var offer = new Offer
            {
                Name=dto.OfferName,
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.StartDate <= DateOnly.FromDateTime(DateTime.Now) && dto.EndDate >= DateOnly.FromDateTime(DateTime.Now),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _promotionRepository.AddAsync(offer);
            _logger.LogInformation("Created new Offer with Id: {OfferId}", offer.Id);
            var result = new GetOfferByIdDto
            {
                Id = offer.Id,
                OfferName=offer.Name,
                DiscountPercentage = offer.DiscountPercentage,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                IsActive = offer.IsActive
            };
            return Result<GetOfferByIdDto>.Success(result);
        }

        // تعديل عرض
        public override async Task<Result> UpdateAsync(int id, UpdateOfferDto dto)

        {
            _logger.LogInformation("Attempting to update Offer with Id: {Id}", id);

            var Offer = await _promotionRepository.GetTableNoTracking()
                                                      .Where(x => x.Id == id)
                                                      .Include(x=>x.PackageTripOffers)
                                                      .FirstOrDefaultAsync();
            if (Offer == null)
                return Result.NotFound($"Offer with Id {id} not found.");

            if (Offer.PackageTripOffers.Any())
                return Result.BadRequest($"Offer related with packageTrip ids {string.Join(',', Offer.PackageTripOffers.Select(x=>x.PackageTripId))} ");
            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Now))   
                return Result.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate < dto.StartDate)
                return Result.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result.BadRequest("DiscountPercentage must be between 1 and 100.");


            Offer.Name =dto.OfferName;
            Offer.DiscountPercentage = dto.DiscountPercentage;
            Offer.StartDate = dto.StartDate;
            Offer.EndDate = dto.EndDate;
            Offer.IsActive = dto.IsActive;
            Offer.UpdatedAt = DateTime.Now;

            await _promotionRepository.UpdateAsync(Offer);
            return Result.Success("Success update");
        }

        public override async Task<Result> DeleteAsync(int id)
        {
            var offer = await _promotionRepository.GetTableNoTracking()
                                                    .Where(x => x.Id == id)
                                                    .Include(x => x.PackageTripOffers)
                                                    .FirstOrDefaultAsync();
            if (offer is null)
                return Result.NotFound($"Offer with Id {id} not found.");

            if (offer.PackageTripOffers.Any())
                return Result.BadRequest($"Cann't Delete Offer Because related with packageTrip ids {string.Join(',', offer.PackageTripOffers.Select(x => x.PackageTripId))} ");

            // التحقق من وجود حجوزات مرتبطة
            var bookings = await _bookingTripRepository.GetTableNoTracking()
                                                       .FirstOrDefaultAsync(b => b.AppliedOfferId == offer.Id);
            if (bookings is not null)
            {
                _logger.LogWarning("Cannot delete Offer with Id: {OfferId} due to active bookings.", id);
                return Result.BadRequest("Cannot delete Offer due to active bookings.");
            }
            await _promotionRepository.DeleteAsync(offer);
            _logger.LogInformation(" deleted promotion with Id: {Id}", id);
            return Result.Success("Offer deleted successfully.");
        }

    }
}
