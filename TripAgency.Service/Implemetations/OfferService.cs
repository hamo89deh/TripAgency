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
            if (dto.StartDate < DateTime.Now.Date)
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
                IsActive = dto.StartDate <= DateTime.Now && dto.EndDate >= DateTime.Now,
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
            if (dto.StartDate < DateTime.Now.Date)   
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

        // استرجاع العروض لرحلة معينة
        public async Task<Result<IEnumerable<GetPackageTripOffersDto>>> GetOffersByPackageTripIdAsync(int packageTripId)

        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking().FirstOrDefaultAsync(d => d.Id == packageTripId);
            if (packageTrip is null)
                return Result<IEnumerable<GetPackageTripOffersDto>>.NotFound($"Not Found PackageTrip with Id :{packageTripId}");

            var PackageTripOffers = await _promotionPackageTripsRepo.GetTableNoTracking()
                                                       .Where(p => p.PackageTripId == packageTripId)
                                                       .Include(p=>p.Offer)
                                                       .ToListAsync();
            if (PackageTripOffers.Count() == 0)
            {
                return Result<IEnumerable<GetPackageTripOffersDto>>.NotFound($"Not Found Any Offers For packageTrip with id :{packageTripId}");
            }
            var result = PackageTripOffers.Select(p => new GetPackageTripOffersDto
            {
                OfferId = p.Offer.Id,
                OfferName = p.Offer.Name,
                IsActive = p.Offer.IsActive,
                DiscountPercentage = p.Offer.DiscountPercentage,
                EndDate = p.Offer.EndDate,
                StartDate = p.Offer.StartDate,
                IsApply= p.IsApply

            });
            return Result<IEnumerable<GetPackageTripOffersDto>>.Success(result);
        }

        // استرجاع عرض صالح للحجز
        public async Task<Result<GetOfferByIdDto>> GetValidOfferAsync(int packageTripId)
        {
            var today = DateTime.Now.Date;
            var promitionPackageTrip = await _promotionPackageTripsRepo.GetTableNoTracking()
                .Include(p=>p.Offer)
                .FirstOrDefaultAsync(p => p.PackageTripId == packageTripId &&
                                          p.IsApply &&
                                          p.Offer.IsActive&&
                                          p.Offer.EndDate >= DateTime.Now &&
                                          p.Offer.StartDate <= DateTime.Now
                                          );
                            
               
            if (promitionPackageTrip is null)
                return Result<GetOfferByIdDto>.NotFound("Not Found Any ValidOffer");
            var result = new GetOfferByIdDto
            {
                Id = promitionPackageTrip.Offer.Id,
                OfferName= promitionPackageTrip.Offer.Name,
                DiscountPercentage = promitionPackageTrip.Offer.DiscountPercentage,
                StartDate = promitionPackageTrip.Offer.StartDate,
                EndDate = promitionPackageTrip.Offer.EndDate,
                IsActive = promitionPackageTrip.Offer.IsActive
            };
            return Result<GetOfferByIdDto>.Success(result);

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
    public class PackageTripOfferService : IPackageTripOffersService
    {
        private readonly IOffersRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OfferService> _logger;

        public IPackageTripOffersRepositoryAsync _packageTripOffersRepo { get; }
        public IBookingTripRepositoryAsync _bookingTripRepository { get; }
        public IPackageTripRepositoryAsync _packageTripRepository { get; }

        public async Task<Result> AddPackageTripOffer(int PackageTirpId, int OfferId)
        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                                                          .Where(x => x.Id == PackageTirpId)
                                                          .FirstOrDefaultAsync();
            if (packageTrip is null)
                return Result.NotFound($"Not Found PackageTrip With Id : {PackageTirpId}");

            var offer = await _offerRepository.GetTableNoTracking()
                                                        .Where(x => x.Id == OfferId)
                                                        .FirstOrDefaultAsync();

            if (offer is null)
                return Result.NotFound($"Not Found Offer With Id : {OfferId}");

            var packageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                                                        .Where(x => x.OfferId == OfferId && x.PackageTripId == PackageTirpId)
                                                        .FirstOrDefaultAsync();
            if (packageTripOffer is not null)
                return Result.BadRequest($"adding Before offer with id :{OfferId} to Package Trip with id :{packageTrip}");

            var packageTripOfferApply = await _packageTripOffersRepo.GetTableNoTracking()
                                            .Where(x => x.PackageTripId == PackageTirpId && x.IsApply)
                                            .FirstOrDefaultAsync();
            if (packageTripOffer is not null)
                return Result.BadRequest($"Found offer Apply before with id :{packageTripOfferApply.OfferId} to Package Trip with id :{packageTrip}");



            await _packageTripOffersRepo.AddAsync(new PackageTripOffers
            {
                OfferId = OfferId,
                PackageTripId = PackageTirpId,
                IsApply = false
            });
            return Result.Success();

        }
        public async Task<Result> DeletePackageTripOffer(int PackageTirpId, int OfferId)
        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                                                           .Where(x => x.Id == PackageTirpId)
                                                           .FirstOrDefaultAsync();
            if (packageTrip is null)
                return Result.NotFound($"Not Found PackageTrip With Id : {PackageTirpId}");

            var offer = await _offerRepository.GetTableNoTracking()
                                                        .Where(x => x.Id == OfferId)
                                                        .FirstOrDefaultAsync();

            if (offer is null)
                return Result.NotFound($"Not Found Offer With Id : {OfferId}");

            var packageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                                                        .Where(x => x.OfferId == OfferId && x.PackageTripId == PackageTirpId)
                                                        .FirstOrDefaultAsync();
            if (packageTripOffer is null)
                return Result.NotFound($" offer with id :{OfferId} Not Related with Package Trip with id :{packageTrip}");

            await _packageTripOffersRepo.DeleteAsync(packageTripOffer);
            return Result.Success();
        }
    }
}
