using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    public class UserCreditServiceClientFactory : IUserCreditServiceClientFactory
    {

        public IUserCreditService UserCreditService
        {
            get
            {
                return new UserCreditServiceClient();
            }
        }
    }
}
