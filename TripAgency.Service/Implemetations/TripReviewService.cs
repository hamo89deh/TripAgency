using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripReview;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class TripReviewService :ReadAndAddAndDeleteService<TripReview,GetTripReviewByIdDto,GetTripReviewsDto,AddTripReviewDto>,ITripReviewService
    {
        private readonly ITripReviewRepositoryAsync _tripReviewRepository;
        private readonly IBookingTripRepositoryAsync _bookingTripRepository;
        private readonly IPackageTripDateRepositoryAsync _packageTripDateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ILogger<TripReviewService> _logger { get; }

        public TripReviewService(
            ITripReviewRepositoryAsync tripReviewRepository,
            IBookingTripRepositoryAsync bookingTripRepository,
            IPackageTripDateRepositoryAsync packageTripDateRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<TripReviewService> logger):base(tripReviewRepository ,mapper)
        {
            _tripReviewRepository = tripReviewRepository;
            _bookingTripRepository = bookingTripRepository;
            _packageTripDateRepository = packageTripDateRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        // إضافة تقييم وتعليق
        public override async Task<Result<GetTripReviewByIdDto>> CreateAsync(AddTripReviewDto dto)
        {
            var user = await _currentUserService.GetUserAsync();
            // التحقق من أن PackageTripDate مكتمل
            var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                .Where(ptd => ptd.Id == dto.PackageTripDateId)
                .FirstOrDefaultAsync();

            if (packageTripDate == null)
            {
                _logger.LogWarning("PackageTripDate with Id {PackageTripDateId} not found.", dto.PackageTripDateId);
                return Result<GetTripReviewByIdDto>.NotFound($"PackageTripDate with Id {dto.PackageTripDateId} not found.");
            }

            if (packageTripDate.Status != PackageTripDateStatus.Completed)
            {
                _logger.LogWarning("PackageTripDate with Id {PackageTripDateId} is not completed. Current status: {Status}",
                    dto.PackageTripDateId, packageTripDate.Status);
                return Result<GetTripReviewByIdDto>.BadRequest("Reviews can only be submitted for completed trips.");
            }
            // التحقق من أن المستخدم لم يقدم تقييمًا مسبقًا
            var existingReview = await _tripReviewRepository.GetTableNoTracking()
                .Where(r => r.PackageTripDateId == dto.PackageTripDateId && r.UserId == user.Id)
                .FirstOrDefaultAsync();

            if (existingReview != null)
            {
                _logger.LogWarning("User {UserId} has already submitted a review for PackageTripDateId {PackageTripDateId}.",
                    user.Id, dto.PackageTripDateId);
                return Result<GetTripReviewByIdDto>.BadRequest($"User has already submitted a review for PackageTripDateId {dto.PackageTripDateId}.");
            }
            // التحقق من أن المستخدم قام بحجز الرحلة
            var bookingExists = await _bookingTripRepository.GetTableNoTracking()
                .AnyAsync(b => b.PackageTripDateId == dto.PackageTripDateId &&
                               b.UserId == user.Id &&
                               b.BookingStatus == BookingStatus.Completed);
            if (!bookingExists)
                return Result<GetTripReviewByIdDto>.BadRequest("You can only review trips you have booked and completed.");

          
            var review = new TripReview
            {
                PackageTripDateId = dto.PackageTripDateId,
                UserId = user.Id,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _tripReviewRepository.AddAsync(review);
            var result = new GetTripReviewByIdDto
            {
                Comment = review.Comment,
                Id = review.Id,
                Rating = review.Rating
            };
            return Result<GetTripReviewByIdDto>.Success(result);
        }

        // استرجاع تقييمات رحلة معينة
        public async Task<Result<IEnumerable<GetTripReviewsDto>>> GetReviewsByPackageTripDateIdAsync(int packageTripDateId)
        {
            var reviews = await _tripReviewRepository.GetTableNoTracking()
                .Where(r => r.PackageTripDateId == packageTripDateId)
                .Include(r => r.User)
                .ToListAsync();

            var result = reviews.Select(r => new GetTripReviewsDto
            {
                Rating = r.Rating,
                Id = r.Id,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                PackageTripDateId = r.PackageTripDateId,
                UserId = r.UserId,
                UserName = r.User.UserName!

            });
            return Result<IEnumerable<GetTripReviewsDto>>.Success(result);
        }

        // استرجاع جميع التقييمات (للإداري)
        public override async Task<Result<IEnumerable<GetTripReviewsDto>>> GetAllAsync()
        {
            var reviews = await _tripReviewRepository.GetTableNoTracking()
                .Include(r => r.User)
                .Include(r => r.PackageTripDate)
                .ToListAsync();

            var result = reviews.Select(r => new GetTripReviewsDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                PackageTripDateId = r.PackageTripDateId,
                UserId = r.UserId,
                UserName = r.User.UserName!

            });
            return Result<IEnumerable<GetTripReviewsDto>>.Success(result);
        }

        // حذف تقييم (للإداري)
        public override async Task<Result> DeleteAsync(int id)
        {
            var review = await _tripReviewRepository.GetByIdAsync(id);
            if (review == null)
                return Result.NotFound($"Review with Id {id} not found.");

            await _tripReviewRepository.DeleteAsync(review);
            return Result.Success("Review deleted successfully.");
        }
        // دالة جديدة للتحقق من إمكانية إضافة تعليق
        public async Task<Result<bool>> CanUserReviewAsync(int packageTripDateId)
        {
            var user = await _currentUserService.GetUserAsync();

            var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                .Where(ptd => ptd.Id == packageTripDateId)
                .FirstOrDefaultAsync();

            if (packageTripDate == null)
            {
                _logger.LogWarning("PackageTripDate with Id {PackageTripDateId} not found.", packageTripDateId);
                return Result<bool>.NotFound($"PackageTripDate with Id {packageTripDateId} not found.");
            }

            if (packageTripDate.Status != PackageTripDateStatus.Completed)
            {
                _logger.LogInformation("PackageTripDate with Id {PackageTripDateId} is not completed. Current status: {Status}",
                    packageTripDateId, packageTripDate.Status);
                return Result<bool>.Success(false);
            }

            var existingReview = await _tripReviewRepository.GetTableNoTracking()
                .AnyAsync(r => r.PackageTripDateId == packageTripDateId && r.UserId == user.Id);

            if (existingReview)
            {
                _logger.LogInformation("User {UserId} has already submitted a review for PackageTripDateId {PackageTripDateId}.",
                    user.Id, packageTripDateId);
                return Result<bool>.Success(false);
            }

            var bookingExists = await _bookingTripRepository.GetTableNoTracking()
                .AnyAsync(b => b.PackageTripDateId == packageTripDateId &&
                               b.UserId == user.Id &&
                               b.BookingStatus == BookingStatus.Completed);

            if (!bookingExists)
            {
                _logger.LogInformation("User {UserId} does not have a completed booking for PackageTripDateId {PackageTripDateId}.",
                    user.Id, packageTripDateId);
                return Result<bool>.Success(false);
            }

            return Result<bool>.Success(true);
        }
        public async Task<decimal?> CalculateAverageRatingAsync(int packageTripId)
        {
            const int minimumReviews = 3;
            var validReviewsCount = await _tripReviewRepository.GetTableNoTracking()
                .Include(p=>p.PackageTripDate)
                .Where(r => r.PackageTripDate.PackageTripId == packageTripId &&  r.Rating >= 1 && r.Rating <= 5)
                .CountAsync();

            if (validReviewsCount < minimumReviews)
                return null;

            return await _tripReviewRepository.GetTableNoTracking()
                .Include(p => p.PackageTripDate)
                .Where(r => r.PackageTripDate.PackageTripId == packageTripId &&  r.Rating >= 1 && r.Rating <= 5)
                .AverageAsync(r => (decimal?)r.Rating);
        }
    }
}
