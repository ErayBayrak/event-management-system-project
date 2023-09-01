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
    public class CompanyManager : ICompanyService
    {
        ICompanyDal _companyDal;

        public CompanyManager(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }

        public void Add(Company company)
        {
            _companyDal.Add(company);
        }

        public void Delete(Company company)
        {
            _companyDal.Delete(company);
        }

        public Company Get(Expression<Func<Company, bool>> filter)
        {
            return _companyDal.Get(filter);
        }

        public List<Company> GetAll(Expression<Func<Company, bool>> filter = null)
        {
            return _companyDal.GetAll(filter);
        }

        public void Update(Company company)
        {
            _companyDal.Update(company);
        }
        public Company GetByMail(string email)
        {
            var values = _companyDal.Get(u => u.Email == email);
            return values;
        }
    }
}
