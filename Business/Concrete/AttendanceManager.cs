using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AttendanceManager : IAttendanceService
    {
        IAttendanceDal _attendanceDal;
        public AttendanceManager(IAttendanceDal attendanceDal)
        {
            _attendanceDal = attendanceDal;
        }

        public void Add(Attendance attendance)
        {
            _attendanceDal.Add(attendance);
        }

        public void Delete(Attendance attendance)
        {
            _attendanceDal.Delete(attendance);
        }

        public Attendance Get(Expression<Func<Attendance, bool>> filter)
        {
            return _attendanceDal.Get(filter);
        }

        public List<Attendance> GetAll(Expression<Func<Attendance, bool>> filter = null)
        {
            return _attendanceDal.GetAll(filter);
        }

        public void Update(Attendance attendance)
        {
            _attendanceDal.Update(attendance);
        }
        public int GetAttendanceCountForEvent(int eventId)
        {
           return _attendanceDal.GetAttendanceCountForEvent(eventId);
        }
    }
}
