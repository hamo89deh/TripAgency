using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.OfferDto;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class OfferService : GenericService<Offer, GetOfferByIdDto, GetOffersDto, AddOfferDto, UpdateOfferDto>, IOfferService
    {
        private readonly IOffersRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OfferService> _logger;

        public IPackageTripOffersRepositoryAsync _packageTripOffersRepo { get; }
        public IBookingTripRepositoryAsync _bookingTripRepository { get; }
        public IPackageTripRepositoryAsync _packageTripRepository { get; }

        public OfferService(IOffersRepositoryAsync offerRepository,
                                IPackageTripOffersRepositoryAsync PackageTripOffersRepo,
                                IBookingTripRepositoryAsync bookingTripRepository,
                                IPackageTripRepositoryAsync packageTripRepository,
                                IMapper mapper,
                                ILogger<OfferService> logger
            ) : base(offerRepository, mapper)
        {
            _offerRepository = offerRepository;
            _packageTripOffersRepo = PackageTripOffersRepo;
            _bookingTripRepository = bookingTripRepository;
            _packageTripRepository = packageTripRepository;
            _mapper = mapper;
            _logger = logger;
        }



        // إضافة عرض جديد
        public override async Task<Result<GetOfferByIdDto>> CreateAsync(AddOfferDto dto)
        {
            // التحقق من صحة التواريخ
            if (dto.StartDate.AddHours(6).Date < DateTime.Now.Date)
                return Result<GetOfferByIdDto>.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate.Date < dto.StartDate.Date)
                return Result<GetOfferByIdDto>.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result<GetOfferByIdDto>.BadRequest("DiscountPercentage must be between 1 and 100.");
            
            var offer = new Offer
            {
                Name=dto.OfferName,
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = new DateTime(dto.StartDate.Year, dto.StartDate.Month, dto.StartDate.Day,0,0,0) ,
                EndDate =   new DateTime(dto.EndDate.Year, dto.EndDate.Month, dto.EndDate.Day, 23, 59, 59),
                IsActive = dto.StartDate <=DateTime.Now && dto.EndDate >= DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _offerRepository.AddAsync(offer);
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

            var offer = await _offerRepository.GetTableNoTracking()
                                                      .Where(x => x.Id == id)
                                                      .Include(x=>x.PackageTripOffers)
                                                      .FirstOrDefaultAsync();
            if (offer == null)
                return Result.NotFound($"Offer with Id {id} not found.");

            if (dto.StartDate.AddHours(6).Date < DateTime.Now.Date)   
                return Result.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate.Date < dto.StartDate.Date)
                return Result.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result.BadRequest("DiscountPercentage must be between 1 and 100.");

            if (offer.PackageTripOffers.Any())
                return Result.BadRequest($"Offer related with packageTrip ids {string.Join(',', offer.PackageTripOffers.Select(x => x.PackageTripId))} ");
           
            // التحقق من وجود حجوزات مرتبطة
            var hasBookings = await _bookingTripRepository.GetTableNoTracking()
                                                       .AnyAsync(b => b.AppliedOfferId == offer.Id);

            if (hasBookings)
            {
                _logger.LogWarning("Cannot Update Offer with Id: {OfferId} due to active bookings.", id);
                return Result.BadRequest("Cannot Update Offer due to active bookings.");
            }
            offer.Name = dto.OfferName;
            offer.DiscountPercentage = dto.DiscountPercentage;
            offer.StartDate = new DateTime(dto.StartDate.Year, dto.StartDate.Month, dto.StartDate.Day, 0, 0, 0);
            offer.EndDate = new DateTime(dto.EndDate.Year, dto.EndDate.Month, dto.EndDate.Day, 23, 59, 59);
            offer.IsActive = dto.IsActive;
            offer.UpdatedAt = DateTime.Now;

            await _offerRepository.UpdateAsync(offer);
            return Result.Success("Success update");
        }

        public override async Task<Result> DeleteAsync(int id)
        {
            var offer = await _offerRepository.GetTableNoTracking()
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
            await _offerRepository.DeleteAsync(offer);
            _logger.LogInformation(" deleted promotion with Id: {Id}", id);
            return Result.Success("Offer deleted successfully.");
        }
    }
}
