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

            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Now))   
                return Result.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate < dto.StartDate)
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
            offer.StartDate = dto.StartDate;
            offer.EndDate = dto.EndDate;
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
        public  async Task<Result> UpdateAsyncw(int id, UpdateOfferDto dto)
        {
            _logger.LogInformation("Attempting to update Offer with Id: {Id}", id);

            var offerData = await _offerRepository.GetTableNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Offer = x,
                    PackageTripOffers = x.PackageTripOffers.ToList()
                })
                .FirstOrDefaultAsync();
            if (offerData == null)
                return Result.NotFound($"Offer with Id {id} not found.");

            var offer = offerData.Offer;

            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Now))
                return Result.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate < dto.StartDate)
                return Result.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result.BadRequest("DiscountPercentage must be between 1 and 100.");

            var hasBookings = await _bookingTripRepository.GetTableNoTracking()
                .AnyAsync(b => b.AppliedOfferId == offer.Id);
            if (hasBookings)
            {
                _logger.LogWarning("Cannot update Offer with Id: {OfferId} due to active bookings.", id);
                return Result.BadRequest("Cannot Update Offer due to active bookings.");
            }

            if (offerData.PackageTripOffers.Any())
            {
                if (dto.DiscountPercentage != offer.DiscountPercentage ||
                    dto.StartDate != offer.StartDate ||
                    dto.EndDate != offer.EndDate)
                {
                    return Result.BadRequest($"Cannot modify DiscountPercentage, StartDate, or EndDate for Offer Id: {id} as it is related to PackageTrip ids {string.Join(',', offerData.PackageTripOffers.Select(x => x.PackageTripId))}.");
                }
            }

            using var transaction = await _offerRepository.BeginTransactionAsync();
            try
            {
                bool isOfferValid = dto.IsActive &&
                                   dto.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                                   dto.StartDate <= DateOnly.FromDateTime(DateTime.Now);
                if (offerData.PackageTripOffers.Any() && !isOfferValid)
                {
                    await _packageTripOffersRepo.GetTableNoTracking()
                        .Where(x => x.OfferId == id && x.IsApply)
                        .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsApply, false));
                    _logger.LogInformation("Disabled IsApply for PackageTripOffers related to Offer Id: {OfferId} due to invalid status or dates.", id);
                }

                offer.Name = dto.OfferName;
                offer.DiscountPercentage = dto.DiscountPercentage;
                offer.StartDate = dto.StartDate;
                offer.EndDate = dto.EndDate;
                offer.IsActive = dto.IsActive;
                offer.UpdatedAt = DateTime.Now;

                await _offerRepository.UpdateAsync(offer);
                await transaction.CommitAsync();
                return Result.Success("Success update");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating Offer with Id: {OfferId}", id);
                return Result.BadRequest("Failed to update offer.");
            }
        }
    }
}
