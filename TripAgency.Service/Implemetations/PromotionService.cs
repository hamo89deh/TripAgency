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
    public class PromotionService : GenericService<Promotion, GetPromotionByIdDto, GetPromotionsDto, AddPromotionDto, UpdatePromotionDto>, IPromotionService
    {
        private readonly IPromotionRepositoryAsync _promotionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PromotionService> _logger;

        public IBookingTripRepositoryAsync _bookingTripRepository { get; }
        public IPackageTripRepositoryAsync _packageTripRepository { get; }

        public PromotionService(IPromotionRepositoryAsync promotionRepository,
                                IBookingTripRepositoryAsync bookingTripRepository,
                                IPackageTripRepositoryAsync packageTripRepository,
                                IMapper mapper,
                                ILogger<PromotionService> logger
            ) : base(promotionRepository, mapper)
        {
            _promotionRepository = promotionRepository;
            _bookingTripRepository = bookingTripRepository;
            _packageTripRepository = packageTripRepository;
            _mapper = mapper;
            _logger = logger;
        }



        // إضافة عرض جديد
        public override async Task<Result<GetPromotionByIdDto>> CreateAsync(AddPromotionDto dto)
        {
            // التحقق من صحة التواريخ
            if (dto.StartDate < DateTime.Now.Date)
                return Result<GetPromotionByIdDto>.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate < dto.StartDate)
                return Result<GetPromotionByIdDto>.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result<GetPromotionByIdDto>.BadRequest("DiscountPercentage must be between 1 and 100.");
            var packageTrip = await _packageTripRepository.GetByIdAsync(dto.PackageTripId);
            if (packageTrip == null)
            {
                _logger.LogWarning("PackageTrip with Id {PackageTripId} not found.", dto.PackageTripId);
                return Result<GetPromotionByIdDto>.NotFound($"PackageTrip with Id {dto.PackageTripId} not found.");
            }
            // التحقق من التواريخ المتداخلة
            var overlappingPromotions = await _promotionRepository.GetTableNoTracking()
                .Where(p => p.PackageTripId == dto.PackageTripId &&
                            p.StartDate.Date <= dto.EndDate.Date &&
                            p.EndDate.Date >= dto.StartDate.Date &&
                            !p.IsDeleted)
                .ToListAsync();
            if (overlappingPromotions.Any())
            {
                _logger.LogWarning("Overlapping promotion dates detected for PackageTripId: {PackageTripId}", dto.PackageTripId);
                return Result<GetPromotionByIdDto>.BadRequest("An existing promotion overlaps with the specified date range.");
            }
            //// إذا كان هناك عرض نشط منتهي، قم بتعطيله

            // التحقق من وجود عرض نشط لنفس PackageTripId
            var activePromotion = await _promotionRepository.GetTableNoTracking()
                .Where(p => p.PackageTripId == dto.PackageTripId && p.IsActive && !p.IsDeleted)
                .FirstOrDefaultAsync();

            if (activePromotion != null && activePromotion.EndDate >= DateTime.Now)
            {
                _logger.LogWarning("An active promotion already exists for PackageTripId: {PackageTripId}", dto.PackageTripId);
                return Result<GetPromotionByIdDto>.BadRequest($"An active promotion already exists for PackageTripId {dto.PackageTripId}.");
            }
            if (activePromotion != null && activePromotion.EndDate < DateTime.Now)
            {
                activePromotion.IsActive = false;
                activePromotion.UpdatedAt = DateTime.Now;
                await _promotionRepository.UpdateAsync(activePromotion);
                _logger.LogInformation("Deactivated promotion with Id: {PromotionId} for PackageTripId: {PackageTripId}", activePromotion.Id, dto.PackageTripId);
            }

            var promotion = new Promotion
            {
                PackageTripId = dto.PackageTripId,
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.StartDate <= DateTime.Now && dto.EndDate >= DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _promotionRepository.AddAsync(promotion);
            _logger.LogInformation("Created new promotion with Id: {PromotionId} for PackageTripId: {PackageTripId}", promotion.Id, dto.PackageTripId);
            var result = new GetPromotionByIdDto
            {
                Id = promotion.Id,
                PackageTripId = promotion.PackageTripId,
                DiscountPercentage = promotion.DiscountPercentage,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive
            };
            return Result<GetPromotionByIdDto>.Success(result);
        }

        // تعديل عرض
        public override async Task<Result> UpdateAsync(int id, UpdatePromotionDto dto)

        {
            _logger.LogInformation("Attempting to update promotion with Id: {Id}", id);

            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
                return Result.NotFound($"Promotion with Id {id} not found.");

            if (dto.StartDate < DateTime.Now.Date)   
                return Result.BadRequest("StartDate must be today or in the future.");
            if (dto.EndDate < dto.StartDate)
                return Result.BadRequest("EndDate must be greater than or equal to StartDate.");
            if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
                return Result.BadRequest("DiscountPercentage must be between 1 and 100.");
            // التحقق من التواريخ المتداخلة مع عروض أخرى
            var overlappingPromotions = await _promotionRepository.GetTableNoTracking()
                .Where(p => p.PackageTripId == promotion.PackageTripId &&
                            p.Id != id &&
                            p.StartDate.Date <= dto.EndDate.Date &&
                            p.EndDate.Date >= dto.StartDate.Date &&
                           !p.IsDeleted)
                .ToListAsync();
            if (overlappingPromotions.Any())
            {
                _logger.LogWarning("Overlapping promotion dates detected for PackageTripId: {PackageTripId}", promotion.PackageTripId);
                return Result.BadRequest("An existing promotion overlaps with the specified date range.");
            }
            promotion.DiscountPercentage = dto.DiscountPercentage;
            promotion.StartDate = dto.StartDate;
            promotion.EndDate = dto.EndDate;
            promotion.IsActive = dto.IsActive;
            promotion.UpdatedAt = DateTime.Now;

            await _promotionRepository.UpdateAsync(promotion);
            return Result.Success("Success update");
        }

        // استرجاع العروض لرحلة معينة
        public async Task<Result<IEnumerable<GetPromotionsDto>>> GetPromotionsByPackageTripIdAsync(int packageTripId)

        {
            var packageTrip = await _packageTripRepository.GetTableNoTracking().FirstOrDefaultAsync(d => d.Id == packageTripId);
            if (packageTrip is null)
                return Result<IEnumerable<GetPromotionsDto>>.NotFound($"Not Found PackageTrip with Id :{packageTripId}");

            var promotions = await _promotionRepository.GetTableNoTracking()
                                                       .Where(p => p.PackageTripId == packageTripId && !p.IsDeleted)
                                                       .ToListAsync();
            if (promotions.Count() == 0)
            {
                return Result<IEnumerable<GetPromotionsDto>>.NotFound($"Not Found Any promotions For packageTrip with id :{packageTripId}");
            }
            var result = promotions.Select(p => new GetPromotionsDto
            {
                Id = p.Id,
                IsActive = p.IsActive,
                DiscountPercentage = p.DiscountPercentage,
                EndDate = p.EndDate,
                StartDate = p.StartDate,
                PackageTripId = packageTripId

            });
            return Result<IEnumerable<GetPromotionsDto>>.Success(result);
        }

        // استرجاع عرض صالح للحجز
        public async Task<Result<GetPromotionByIdDto>> GetValidPromotionAsync(int packageTripId)
        {
            var today = DateTime.Now.Date;
            var promition = await _promotionRepository.GetTableNoTracking()
                .Where(p => p.PackageTripId == packageTripId &&
                            p.IsActive &&
                            !p.IsDeleted &&
                            p.StartDate.Date <= today &&
                            p.EndDate.Date >= today)
                .FirstOrDefaultAsync();
            if (promition is null)
                return Result<GetPromotionByIdDto>.NotFound("Not Found Any ValidPromotion");
            var result = new GetPromotionByIdDto
            {
                Id = promition.Id,
                DiscountPercentage = promition.DiscountPercentage,
                StartDate = promition.StartDate,
                EndDate = promition.EndDate,
                IsActive = promition.IsActive,
                PackageTripId = packageTripId,
            };
            return Result<GetPromotionByIdDto>.Success(result);

        }
        public override async Task<Result> DeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to soft delete promotion with Id: {Id}", id);

            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null || promotion.IsDeleted)
            {
                _logger.LogWarning("Promotion with Id {Id} not found or is already deleted.", id);
                return Result.NotFound($"Promotion with Id {id} not found.");
            }

            // التحقق من وجود حجوزات مرتبطة
            var bookings = await _bookingTripRepository.GetTableNoTracking()
                                                       .Include(b => b.PackageTripDate)
                                                       .Where(b => b.AppliedPromotionId == promotion.Id && b.PackageTripDate.PackageTripId == promotion.PackageTripId)
                                                       .FirstOrDefaultAsync();
            if (bookings is not null)
            {
                _logger.LogWarning("Cannot delete promotion with Id: {PromotionId} due to active bookings.", id);
                return Result.BadRequest("Cannot delete promotion due to active bookings.");
            }

            promotion.IsDeleted = true;
            promotion.UpdatedAt = DateTime.Now;
            await _promotionRepository.UpdateAsync(promotion);
            _logger.LogInformation("Soft deleted promotion with Id: {Id}", id);
            return Result.Success("Promotion soft deleted successfully.");
        }

    }
}
