using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Enums
{
    public enum PackageTripDataStatus
    {
        Draft = 0,          // مسودة (قبل النشر)
        Published = 1,      // منشورة ومتاحة للحجز
        BookingClosed = 2,  // الحجز مغلق (قبل الرحلة بوقت قصير)
        Full =3,            // ممتلة بسبب الحجز 
        Ongoing =4,         // جارية (الرحلة قيد التنفيذ)
        Completed =5,       // مكتملة (انتهت بنجاح)
        Cancelled =6        // ملغاة (من قبل الإدارة)
      
    }
}
