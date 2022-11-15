using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    public interface IUserDataAccessRepository
    {
        void AddUser(User user);
    }
}
