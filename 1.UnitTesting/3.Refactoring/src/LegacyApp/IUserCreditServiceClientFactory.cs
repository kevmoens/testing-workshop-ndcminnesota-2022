using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    public partial interface IUserCreditServiceAdapter : IUserCreditService, IDisposable
    { }
    public interface IUserCreditServiceClientFactory
    {
        public IUserCreditService UserCreditService { get; }
    }
}
