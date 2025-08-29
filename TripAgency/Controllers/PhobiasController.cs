using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Phobia.Commands;
using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhobiasController : ControllerBase
    {
        public PhobiasController(IPhobiaService phobiaService, IMapper mapper)
        {
            _phobiaService = phobiaService;
            _mapper = mapper;
        }

        public IPhobiaService _phobiaService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetPhobiasDto>>> GetPhobias()
        {
            var phobiasResult = await _phobiaService.GetAllAsync();
            if (!phobiasResult.IsSuccess)
                return this.ToApiResult(phobiasResult);
            return ApiResult<IEnumerable<GetPhobiasDto>>.Ok(phobiasResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetPhobiaByIdDto>> GetPhobiaById(int id)
        {
            var phobiaResult = await _phobiaService.GetByIdAsync(id);
            if (!phobiaResult.IsSuccess)
                return this.ToApiResult(phobiaResult);
            return ApiResult<GetPhobiaByIdDto>.Ok(phobiaResult.Value!);
        }

        [HttpPost]
        public async Task<ApiResult<GetPhobiaByIdDto>> AddPhobia(AddPhobiaDto phobia)
        {
            var phobiaResult = await _phobiaService.CreateAsync(phobia);
            if (!phobiaResult.IsSuccess)
            {
                return this.ToApiResult(phobiaResult);
            }
            return ApiResult<GetPhobiaByIdDto>.Created(phobiaResult.Value!);
        }
        [HttpPut("Id")]
        public async Task<ApiResult<string>> UpdatePhobia(int Id ,UpdatePhobiaDto updatePhobia)
        {
            var phobiaResult = await _phobiaService.UpdateAsync(Id, updatePhobia);
            if (!phobiaResult.IsSuccess)
                return this.ToApiResult<string>(phobiaResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeletePhobia(int Id)
        {
            var phobiaResult = await _phobiaService.DeleteAsync(Id);
            if (!phobiaResult.IsSuccess)
                return this.ToApiResult<string>(phobiaResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
