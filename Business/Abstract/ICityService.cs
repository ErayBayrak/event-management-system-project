﻿using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICityService
    {
        void Add(City city);
        void Delete(City city);
        void Update(City city);
        List<City> GetAll(Expression<Func<City, bool>> filter = null);
        City Get(Expression<Func<City, bool>> filter);
    }
}
