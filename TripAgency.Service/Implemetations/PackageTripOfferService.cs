using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.OfferDto;

namespace TripAgency.Service.Implementations
{
    public class PackageTripOfferService : IPackageTripOffersService
    {
        private readonly IOffersRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OfferService> _logger;

        public PackageTripOfferService(IOffersRepositoryAsync offerRepository, IMapper mapper, ILogger<OfferService> logger, IPackageTripOffersRepositoryAsync packageTripOffersRepo, IBookingTripRepositoryAsync bookingTripRepository, IPackageTripRepositoryAsync packageTripRepository)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
            _logger = logger;
            _packageTripOffersRepo = packageTripOffersRepo;
            _bookingTripRepository = bookingTripRepository;
            _packageTripRepository = packageTripRepository;
        }

        public IPackageTripOffersRepositoryAsync _packageTripOffersRepo { get; }
        public IBookingTripRepositoryAsync _bookingTripRepository { get; }
        public IPackageTripRepositoryAsync _packageTripRepository { get; }

        public async Task<Result> AddPackageTripOffer(int packageTripId, int offerId)
        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                                                          .Where(x => x.Id == packageTripId)
                                                          .FirstOrDefaultAsync();
            if (packageTrip is null)
                return Result.NotFound($"Not Found PackageTrip With Id : {packageTripId}");

            var offer = await _offerRepository.GetTableNoTracking()
                                                        .Where(x => x.Id == offerId)
                                                        .FirstOrDefaultAsync();

            if (offer is null)
                return Result.NotFound($"Not Found Offer With Id : {offerId}");

            var packageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                                                        .Where(x => x.OfferId == offerId && x.PackageTripId == packageTripId)
                                                        .FirstOrDefaultAsync();
            if (packageTripOffer is not null)
                return Result.BadRequest($"Offer with Id: {offerId} is already associated with PackageTrip Id: {packageTripId}");


            // التحقق من وجود عرض نشط آخر
            var activePackageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                .Include(x => x.Offer)
                .Where(x => x.PackageTripId == packageTripId && x.IsApply)
                .FirstOrDefaultAsync();
            if (activePackageTripOffer != null)
                return Result.BadRequest($"Another offer with Id: {activePackageTripOffer.OfferId} is already applied to PackageTrip Id: {packageTripId}. Please cancel the existing offer first.");


            bool isApply = activePackageTripOffer == null && offer.IsActive && offer.EndDate >= DateOnly.FromDateTime(DateTime.Now) && offer.StartDate <= DateOnly.FromDateTime(DateTime.Now);


            await _packageTripOffersRepo.AddAsync(new PackageTripOffers
            {
                OfferId = offerId,
                PackageTripId = packageTripId,
                IsApply = isApply
            });
            return Result.Success();

        }
        public async Task<Result> CancelAppliedOfferAsync(int packageTripId)
        {
            // التحقق من وجود PackageTrip
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                .Where(x => x.Id == packageTripId)
                .FirstOrDefaultAsync();
            if (packageTrip == null)
                return Result.NotFound($"Not Found PackageTrip With Id: {packageTripId}");

            // البحث عن العرض النشط
            var activePackageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                .Include(x => x.Offer)
                .Where(x => x.PackageTripId == packageTripId && x.IsApply)
                .FirstOrDefaultAsync();
            if (activePackageTripOffer == null)
                return Result.NotFound($"No active offer found for PackageTrip Id: {packageTripId}");

            // إلغاء تطبيق العرض
            activePackageTripOffer.IsApply = false;
            await _packageTripOffersRepo.UpdateAsync(activePackageTripOffer);

            return Result.Success("Offer cancelled successfully.");
        }
        public async Task<Result> DeletePackageTripOffer(int packageTripId, int offerId)
        {

            // التحقق من وجود PackageTrip
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                .Where(x => x.Id == packageTripId)
                .FirstOrDefaultAsync();
            if (packageTrip == null)
                return Result.NotFound($"Not Found PackageTrip With Id: {packageTripId}");

            // التحقق من وجود Offer
            var offer = await _offerRepository.GetTableNoTracking()
                .Where(x => x.Id == offerId)
                .FirstOrDefaultAsync();
            if (offer == null)
                return Result.NotFound($"Not Found Offer With Id: {offerId}");

            // التحقق من وجود العلاقة
            var packageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                .Where(x => x.OfferId == offerId && x.PackageTripId == packageTripId)
                .FirstOrDefaultAsync();
            if (packageTripOffer == null)
                return Result.NotFound($"Offer with Id: {offerId} is not related to PackageTrip with Id: {packageTripId}");

            // التحقق من الحجوزات المرتبطة
            var bookings = await _bookingTripRepository.GetTableNoTracking().Include(p => p.PackageTripDate)
                .Where(b => b.AppliedOfferId == offerId && b.PackageTripDate.PackageTripId == packageTripId)
                .FirstOrDefaultAsync();
            if (bookings != null)
                return Result.BadRequest($"Cannot delete offer because it is associated with active bookings.");

            await _packageTripOffersRepo.DeleteAsync(packageTripOffer);
            return Result.Success("Offer removed successfully.");
        }
        public async Task<Result> ReapplyOfferAsync(int packageTripId, int offerId)
        {
            // التحقق من وجود PackageTrip
            var packageTrip = await _packageTripRepository.GetTableNoTracking()
                .Where(x => x.Id == packageTripId)
                .FirstOrDefaultAsync();
            if (packageTrip == null)
                return Result.NotFound($"Not Found PackageTrip With Id: {packageTripId}");

            // التحقق من وجود Offer
            var offer = await _offerRepository.GetTableNoTracking()
                .Where(x => x.Id == offerId)
                .FirstOrDefaultAsync();
            if (offer == null)
                return Result.NotFound($"Not Found Offer With Id: {offerId}");

            // التحقق من وجود العلاقة
            var packageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                .Where(x => x.OfferId == offerId && x.PackageTripId == packageTripId)
                .FirstOrDefaultAsync();
            if (packageTripOffer == null)
                return Result.NotFound($"Offer with Id: {offerId} is not associated with PackageTrip Id: {packageTripId}");

            // التحقق من أن العرض ليس مطبقًا بالفعل
            if (packageTripOffer.IsApply)
                return Result.BadRequest($"Offer with Id: {offerId} is already applied to PackageTrip Id: {packageTripId}");

            // التحقق من وجود عرض نشط آخر
            var activePackageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                .Include(x => x.Offer)
                .Where(x => x.PackageTripId == packageTripId && x.IsApply)
                .FirstOrDefaultAsync();
            if (activePackageTripOffer != null)
                return Result.BadRequest($"Another offer with Id: {activePackageTripOffer.OfferId} is already applied to PackageTrip Id: {packageTripId}. Please cancel the existing offer first.");

            // التحقق من صلاحية العرض
            if (!offer.IsActive || offer.EndDate < DateOnly.FromDateTime(DateTime.Now) || offer.StartDate > DateOnly.FromDateTime(DateTime.Now))
                return Result.BadRequest($"Offer with Id: {offerId} is not valid for reapplication (inactive or expired).");

            // إعادة تطبيق العرض
            packageTripOffer.IsApply = true;
            await _packageTripOffersRepo.UpdateAsync(packageTripOffer);
            _logger.LogInformation("Reapplied Offer Id: {OfferId} to PackageTrip Id: {PackageTripId}", offerId, packageTripId);
            return Result.Success("Offer reapplied successfully.");
        }
        // استرجاع العروض لرحلة معينة
        public async Task<Result<IEnumerable<GetPackageTripOffersDto>>> GetOffersByPackageTripIdAsync(int packageTripId)

        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking().FirstOrDefaultAsync(d => d.Id == packageTripId);
            if (packageTrip is null)
                return Result<IEnumerable<GetPackageTripOffersDto>>.NotFound($"Not Found PackageTrip with Id :{packageTripId}");

            var PackageTripOffers = await _packageTripOffersRepo.GetTableNoTracking()
                                                       .Where(p => p.PackageTripId == packageTripId)
                                                       .Include(p => p.Offer)
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
                IsApply = p.IsApply

            });
            return Result<IEnumerable<GetPackageTripOffersDto>>.Success(result);
        }
        // استرجاع عرض صالح للحجز
        public async Task<Result<GetOfferByIdDto>> GetValidOfferAsync(int packageTripId)
        {
            var today = DateTime.Now.Date;
            var PackageTripOffer = await _packageTripOffersRepo.GetTableNoTracking()
                .Include(p => p.Offer)
                .FirstOrDefaultAsync(p => p.PackageTripId == packageTripId &&
                                          p.IsApply &&
                                          p.Offer.IsActive &&
                                          p.Offer.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                                          p.Offer.StartDate <= DateOnly.FromDateTime(DateTime.Now)
                                          );


            if (PackageTripOffer is null)
                return Result<GetOfferByIdDto>.NotFound("Not Found Any ValidOffer");
            var result = new GetOfferByIdDto
            {
                Id = PackageTripOffer.Offer.Id,
                OfferName = PackageTripOffer.Offer.Name,
                DiscountPercentage = PackageTripOffer.Offer.DiscountPercentage,
                StartDate = PackageTripOffer.Offer.StartDate,
                EndDate = PackageTripOffer.Offer.EndDate,
                IsActive = PackageTripOffer.Offer.IsActive
            };
            return Result<GetOfferByIdDto>.Success(result);

        }
    }
}
