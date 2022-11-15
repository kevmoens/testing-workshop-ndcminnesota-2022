using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    internal class UserDataAccessRepository : IUserDataAccessRepository
    {
        public UserDataAccessRepository()
        {
        }

        public void AddUser(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}
