using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripType.Commands;
using TripAgency.Service.Feature.PackageTripType.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class PackageTripTypesController : ControllerBase
    {
        public PackageTripTypesController(IPackageTripTypesService packageTripTypeService, IMapper mapper)
        {
            _packageTripTypeService = packageTripTypeService;
            _mapper = mapper;
        }

        public IPackageTripTypesService _packageTripTypeService { get; }
        public IMapper _mapper { get; }

        [HttpGet("{PackageTripId}")]
        public async Task<ApiResult<GetPackageTripTypesDto>> GetPackageTripTypesDto(int PackageTripId)
        {
            var PackageTripTypesResult = await _packageTripTypeService.GetPackageTripTypesAsync(PackageTripId);
            if (!PackageTripTypesResult.IsSuccess)
                return this.ToApiResult(PackageTripTypesResult);
            return ApiResult<GetPackageTripTypesDto>.Ok(PackageTripTypesResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddPackageTripTypes(AddPackageTripTypesDto packageTripTypesDto)
        {
            var AddPackageTripTypesResult = await _packageTripTypeService.AddPackageTripTypes(packageTripTypesDto);
            if (!AddPackageTripTypesResult.IsSuccess)
            {
                return this.ToApiResult<string>(AddPackageTripTypesResult);
            }
            return ApiResult<string>.Created(AddPackageTripTypesResult.Message!);
        }

        [HttpDelete]
        public async Task<ApiResult<string>> DeletePackageTripType(int PackageTripId, int TripTypeID)
        {
            var DeletePackageTripTypeResult = await _packageTripTypeService.DeletePackageTripType(PackageTripId, TripTypeID);
            if (!DeletePackageTripTypeResult.IsSuccess)
                return this.ToApiResult<string>(DeletePackageTripTypeResult);
            return ApiResult<string>.Ok("Success Delete");
        }

    }
}
