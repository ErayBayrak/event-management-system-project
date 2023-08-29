﻿using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Linq.Expressions;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;
        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }
        public void Add(User user)
        {
            _userDal.Add(user);
        }

        public void Delete(User user)
        {
            _userDal.Delete(user);
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            return _userDal.Get(filter);
        }

        public List<User> GetUsers(Expression<Func<User, bool>> filter = null)
        {
            return _userDal.GetAll(filter);
        }

        public void Update(User user)
        {
            _userDal.Update(user);

        }
        public User GetByMail(string email)
        {
            var values = _userDal.Get(u => u.Email == email);
            return values;
        }

    }
}