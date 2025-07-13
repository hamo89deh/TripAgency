using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Enums;

namespace TripAgency.Data.NewFolder1
{
    public class Global
    {
        public static PackageTripDataStatusDto ConvertToDto(PackageTripDataStatus status)
        {
            switch (status)
            {
                case PackageTripDataStatus.Published:
                    return PackageTripDataStatusDto.Published;
                case PackageTripDataStatus.BookingClosed:
                    return PackageTripDataStatusDto.BookingClosed;
                case PackageTripDataStatus.Cancelled:
                    return PackageTripDataStatusDto.Cancelled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }  
        
        public static PackageTripDataStatus ConvertDtoToEntity(PackageTripDataStatusDto status)
        {
            switch (status)
            {
                case PackageTripDataStatusDto.Published:
                    return PackageTripDataStatus.Published;
                case PackageTripDataStatusDto.BookingClosed:
                    return PackageTripDataStatus.BookingClosed;
                case PackageTripDataStatusDto.Cancelled:
                    return PackageTripDataStatus.Cancelled;              
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}
