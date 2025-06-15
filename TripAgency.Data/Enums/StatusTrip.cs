using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Enums
{
    public enum TripDataStatus
    {
        Available = 0,
        Completed,
        Cancelled,
        Planned
    }
    public enum BookingStatus
    {
        Pending = 0,
        Completed,
        Cancelled
    } 
    public enum PaymentStatus
    {
        Pending = 0,
        Completed,
        Cancelled
    }
}
