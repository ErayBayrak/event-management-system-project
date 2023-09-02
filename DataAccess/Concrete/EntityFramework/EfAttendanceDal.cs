using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfAttendanceDal : EfEntityRepositoryBase<Attendance, Context>, IAttendanceDal
    {
        public int GetAttendanceCountForEvent(int eventId)
        {
            using (Context context = new Context())
            {
                var countOfAttendances = context.Attendances.Count(a=>a.EventId == eventId);
                return countOfAttendances;
            }
        }
    }
}
