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
        public static enUpdatePackageTripDataStatusDto ConvertPackageTripDataStatusToenUpdatePackageTripDataStatusDto(PackageTripDataStatus status)
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
        
        public static PackageTripDataStatus ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(enUpdatePackageTripDataStatusDto status)
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

        public static enPackageTripDataStatusDto ConvertPackageTripDataStatusToEnPackageTripDataStatusDto(PackageTripDataStatus status)
        {
            switch (status)
            {
                case PackageTripDataStatus.Draft:
                    return enPackageTripDataStatusDto.Draft;

                case PackageTripDataStatus.Published:
                    return enPackageTripDataStatusDto.Published;

                case PackageTripDataStatus.BookingClosed:
                    return enPackageTripDataStatusDto.BookingClosed;

                case PackageTripDataStatus.Full:
                    return enPackageTripDataStatusDto.Full;

                case PackageTripDataStatus.Cancelled:
                    return enPackageTripDataStatusDto.Cancelled;   
                               
                case PackageTripDataStatus.Completed:
                    return enPackageTripDataStatusDto.Completed;

                case PackageTripDataStatus.Ongoing:
                    return enPackageTripDataStatusDto.Ongoing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public static PackageTripDataStatus ConvertEnPackageTripDataStatusDtoToPackageTripDataStatus(enPackageTripDataStatusDto status)
        {
            switch (status)
            {
                case enPackageTripDataStatusDto.Draft:
                    return PackageTripDataStatus.Draft;

                case enPackageTripDataStatusDto.Published:
                    return PackageTripDataStatus.Published;

                case enPackageTripDataStatusDto.BookingClosed:
                    return PackageTripDataStatus.BookingClosed;

                case enPackageTripDataStatusDto.Full:
                    return PackageTripDataStatus.Full;

                case enPackageTripDataStatusDto.Cancelled:
                    return PackageTripDataStatus.Cancelled;

                case enPackageTripDataStatusDto.Completed:
                    return PackageTripDataStatus.Completed;

                case enPackageTripDataStatusDto.Ongoing:
                    return PackageTripDataStatus.Ongoing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}
