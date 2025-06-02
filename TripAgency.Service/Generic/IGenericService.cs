namespace TripAgency.Service.Generic
{
    public interface IGenericService<T, GetByIdDto, GetALlDto, AddDto, UpdateDto> : IReadService<T, GetByIdDto, GetALlDto>  ,
                                                                                    IAddService<T , AddDto,GetByIdDto> ,
                                                                                    IUpdateService<T , UpdateDto> ,
                                                                                    IDeleteService<T>
    {
    }

}
