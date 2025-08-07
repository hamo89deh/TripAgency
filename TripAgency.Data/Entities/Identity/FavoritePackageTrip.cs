using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Entities.Identity
{
    public class FavoritePackageTrip
    {
        public int UserId { get; set; }
        public int PackageTripId {  get; set; }
        public  PackageTrip PackageTrip { get; set; }
        public  User User { get; set; }
    }
}
