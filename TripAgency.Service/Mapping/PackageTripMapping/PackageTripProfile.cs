using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.PackageTripMapping
{
    public partial class PackageTripProfile :Profile
    {
        public PackageTripProfile()
        {
            GetPackageTripByIdMapping();
            GetPackageTripsMapping();
            AddPackageTripMpping();
            UpdatePackageTripMpping();
        }
    }
}
