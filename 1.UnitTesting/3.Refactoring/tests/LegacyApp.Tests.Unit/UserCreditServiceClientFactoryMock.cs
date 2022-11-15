using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp.Tests.Unit
{
    public class UserCreditServiceClientFactoryMock : IUserCreditServiceClientFactory
    {
        public IUserCreditService UserCreditService => Substitute.For<IUserCreditServiceAdapter>();
    }
}
