using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Hotel.Commands;
using TripAgency.Service.Feature.Hotel.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class HotelService : GenericService<Hotel, GetHotelByIdDto, GetHotelsDto, AddHotelDto, UpdateHotelDto>, IHotelService
    {
        private IHotelRepositoryAsync _hotelRepository { get; set; }
        public ICityRepositoryAsync _cityRepositoryAsync { get; }
        public IMapper _mapper { get; }

        public HotelService(IHotelRepositoryAsync hotelRepository,ICityRepositoryAsync cityRepository,
                            IMapper mapper
                           ) : base(hotelRepository, mapper)
        {
            _hotelRepository = hotelRepository;
            _cityRepositoryAsync = cityRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetHotelByIdDto>> GetHotelByNameAsync(string name)
        {
           var hotel = await _hotelRepository.GetHotelByName(name);
            if (hotel is null)
                return Result<GetHotelByIdDto>.NotFound($"Not Found Hotel By Name : {name}");
            var resultHotel =  _mapper.Map<GetHotelByIdDto>(hotel);
            return Result<GetHotelByIdDto>.Success(resultHotel);
        }
        public override async Task<Result<GetHotelByIdDto>> CreateAsync(AddHotelDto AddDto)
        {
            var city = await _cityRepositoryAsync.GetByIdAsync(AddDto.CityId);
            if(city is null)
                return Result<GetHotelByIdDto>.NotFound($"Not Found City By Id : {AddDto.CityId}");
            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdateHotelDto UpdateDto)
        {
            var city = await _cityRepositoryAsync.GetByIdAsync(UpdateDto.CityId);
            if (city is null)
                return Result.NotFound($"Not Found City By Id : {UpdateDto.CityId}");
            return await base.UpdateAsync(id, UpdateDto);
        }
    }
}
