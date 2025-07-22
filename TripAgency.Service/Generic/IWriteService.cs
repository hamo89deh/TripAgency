using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Generic
{
    public interface IWriteService<T, AddDto, UpdateDto, GetByIdDto> : IAddService<T,AddDto , GetByIdDto> , IUpdateService<T,UpdateDto >
    {
    }
    
}
