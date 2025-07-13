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
        public static enUpdatePackageTripDataStatusDto ConvertToDto(PackageTripDataStatus status)
        {
            switch (status)
            {
                case PackageTripDataStatus.Published:
                    return enUpdatePackageTripDataStatusDto.Published;
                case PackageTripDataStatus.BookingClosed:
                    return enUpdatePackageTripDataStatusDto.BookingClosed;
                case PackageTripDataStatus.Cancelled:
                    return enUpdatePackageTripDataStatusDto.Cancelled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }  
        
        public static PackageTripDataStatus ConvertDtoToEntity(enUpdatePackageTripDataStatusDto status)
        {
            switch (status)
            {
                case enUpdatePackageTripDataStatusDto.Published:
                    return PackageTripDataStatus.Published;
                case enUpdatePackageTripDataStatusDto.BookingClosed:
                    return PackageTripDataStatus.BookingClosed;
                case enUpdatePackageTripDataStatusDto.Cancelled:
                    return PackageTripDataStatus.Cancelled;              
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}
