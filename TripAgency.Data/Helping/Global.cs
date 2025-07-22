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
        public static enUpdatePackageTripDataStatusDto ConvertPackageTripDataStatusToenUpdatePackageTripDataStatusDto(PackageTripDateStatus status)
        {
            switch (status)
            {
                case PackageTripDateStatus.Published:
                    return enUpdatePackageTripDataStatusDto.Published;
                case PackageTripDateStatus.BookingClosed:
                    return enUpdatePackageTripDataStatusDto.BookingClosed;
                case PackageTripDateStatus.Cancelled:
                    return enUpdatePackageTripDataStatusDto.Cancelled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }  
        
        public static PackageTripDateStatus ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(enUpdatePackageTripDataStatusDto status)
        {
            switch (status)
            {
                case enUpdatePackageTripDataStatusDto.Published:
                    return PackageTripDateStatus.Published;
                case enUpdatePackageTripDataStatusDto.BookingClosed:
                    return PackageTripDateStatus.BookingClosed;
                case enUpdatePackageTripDataStatusDto.Cancelled:
                    return PackageTripDateStatus.Cancelled;              
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public static enPackageTripDataStatusDto ConvertPackageTripDataStatusToEnPackageTripDataStatusDto(PackageTripDateStatus status)
        {
            switch (status)
            {
                case PackageTripDateStatus.Draft:
                    return enPackageTripDataStatusDto.Draft;

                case PackageTripDateStatus.Published:
                    return enPackageTripDataStatusDto.Published;

                case PackageTripDateStatus.BookingClosed:
                    return enPackageTripDataStatusDto.BookingClosed;

                case PackageTripDateStatus.Full:
                    return enPackageTripDataStatusDto.Full;

                case PackageTripDateStatus.Cancelled:
                    return enPackageTripDataStatusDto.Cancelled;   
                               
                case PackageTripDateStatus.Completed:
                    return enPackageTripDataStatusDto.Completed;

                case PackageTripDateStatus.Ongoing:
                    return enPackageTripDataStatusDto.Ongoing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public static PackageTripDateStatus ConvertEnPackageTripDataStatusDtoToPackageTripDataStatus(enPackageTripDataStatusDto status)
        {
            switch (status)
            {
                case enPackageTripDataStatusDto.Draft:
                    return PackageTripDateStatus.Draft;

                case enPackageTripDataStatusDto.Published:
                    return PackageTripDateStatus.Published;

                case enPackageTripDataStatusDto.BookingClosed:
                    return PackageTripDateStatus.BookingClosed;

                case enPackageTripDataStatusDto.Full:
                    return PackageTripDateStatus.Full;

                case enPackageTripDataStatusDto.Cancelled:
                    return PackageTripDateStatus.Cancelled;

                case enPackageTripDataStatusDto.Completed:
                    return PackageTripDateStatus.Completed;

                case enPackageTripDataStatusDto.Ongoing:
                    return PackageTripDateStatus.Ongoing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}
