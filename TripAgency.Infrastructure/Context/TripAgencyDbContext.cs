using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Infrastructure.Context
{
    public class TripAgencyDbContext : DbContext
    {
        public TripAgencyDbContext(DbContextOptions options)  : base(options)
        {
            
        }

    }
}
