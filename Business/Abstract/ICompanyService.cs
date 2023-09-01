using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICompanyService
    {
        void Add(Company company);
        void Delete(Company company);
        void Update(Company company);
        List<Company> GetAll(Expression<Func<Company, bool>> filter = null);
        Company Get(Expression<Func<Company, bool>> filter);
        Company GetByMail(string email);
    }
}
