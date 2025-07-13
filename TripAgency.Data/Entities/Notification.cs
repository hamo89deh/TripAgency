using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TripAgency.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int PackageTripDateId { get; set; }
        public string Title {  get; set; }  
        public string Message { get; set; } 
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

    }
}
