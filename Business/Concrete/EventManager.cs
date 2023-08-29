using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class EventManager : IEventService
    {
        IEventDal _eventDal;
        public EventManager(IEventDal eventDal)
        {
            _eventDal = eventDal;
        }
        public void Add(Event entity)
        {
            _eventDal.Add(entity);
        }

        public void Delete(Event entity)
        {
            _eventDal.Delete(entity);
        }

        public Event Get(Expression<Func<Event, bool>> filter)
        {
            return _eventDal.Get(filter);
        }

        public List<Event> GetAll(Expression<Func<Event, bool>> filter = null)
        {
            return _eventDal.GetAll(filter);
        }

        public void Update(Event entity)
        {
            _eventDal.Update(entity);
        }
    }
}
