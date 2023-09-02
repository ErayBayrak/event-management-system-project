using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class EfDataAccessModule
    {
        public static void AddScopeDAL(this IServiceCollection services)
        {
            services.AddScoped<IUserDal, EfUserDal>();
            services.AddScoped<ICategoryDal, EfCategoryDal>();
            services.AddScoped<ICityDal, EfCityDal>();
            services.AddScoped<IEventDal, EfEventDal>();
            services.AddScoped<IAttendanceDal, EfAttendanceDal>();
            services.AddScoped<ICompanyDal, EfCompanyDal>();
        }
    }
}
