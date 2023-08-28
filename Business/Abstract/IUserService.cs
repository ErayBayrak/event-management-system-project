using Entities.Concrete;
using System.Linq.Expressions;

namespace Business.Abstract
{
    public interface IUserService
    {
        void Add(User user);
        void Delete(User user);
        void Update(User user);
        User Get(Expression<Func<User, bool>> filter);
        List<User> GetUsers(Expression<Func<User, bool>> filter = null);
        User GetByMail(string email);
    }
}
