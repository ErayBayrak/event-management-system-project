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
            Context context = new Context();
            return context.Attendances.Count(a => a.EventId == eventId);
        }
    }
}
