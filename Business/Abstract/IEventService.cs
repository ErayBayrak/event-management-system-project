using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IEventService
    {
        void Add(Event entity);
        void Delete(Event entity);
        void Update(Event entity);
        List<Event> GetAll(Expression<Func<Event, bool>> filter = null);
        Event Get(Expression<Func<Event, bool>> filter);
    }
}
