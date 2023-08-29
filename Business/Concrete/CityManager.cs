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
    public class CityManager : ICityService
    {
        ICityDal _cityDal;
        public CityManager(ICityDal cityDal)
        {
            _cityDal = cityDal;
        }
        public void Add(City city)
        {
            _cityDal.Add(city);
        }

        public void Delete(City city)
        {
            _cityDal.Delete(city);
        }

        public City Get(Expression<Func<City, bool>> filter)
        {
            return _cityDal.Get(filter);
        }

        public List<City> GetAll(Expression<Func<City, bool>> filter = null)
        {
            return _cityDal.GetAll(filter);
        }

        public void Update(City city)
        {
            _cityDal.Update(city);
        }
    }
}
