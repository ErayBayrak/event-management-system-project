using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAttendanceService
    {
        void Add(Attendance attendance);
        void Delete(Attendance attendance);
        void Update(Attendance attendance);
        List<Attendance> GetAll(Expression<Func<Attendance, bool>> filter = null);
        Attendance Get(Expression<Func<Attendance, bool>> filter);
        int GetAttendanceCountForEvent(int eventId);
        
    }
}
