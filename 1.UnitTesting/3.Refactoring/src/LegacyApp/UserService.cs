using System;

namespace LegacyApp
{
    public class UserService
    {
        public readonly IClientRepository clientRepository;
        public readonly IUserCreditServiceClientFactory userCreditServiceClientFactory;
        public readonly IUserDataAccessRepository userDataAccess;
        public UserService()
        {
            this.clientRepository = new ClientRepository();
            this.userCreditServiceClientFactory = new UserCreditServiceClientFactory();
            this.userDataAccess = new UserDataAccessRepository();
        }
        public UserService(IClientRepository clientRepository, IUserCreditServiceClientFactory userCreditServiceClientFactory, IUserDataAccessRepository userDataAccess)
        {
            this.clientRepository = clientRepository;
            this.userCreditServiceClientFactory = userCreditServiceClientFactory;
            this.userDataAccess = userDataAccess;
        }
        public bool AddUser(string firname, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            if (string.IsNullOrEmpty(firname) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            if (age < 21)
            {
                return false;
            }

            //var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);

            var user = new User
           {
               Client = client,
               DateOfBirth = dateOfBirth,
               EmailAddress = email,
               Firstname = firname,
               Surname = surname
           };

            if (client.Name == "VeryImportantClient")
            {
                // Skip credit check
                user.HasCreditLimit = false;
            }
            else if (client.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                user.HasCreditLimit = true;
                using (var userCreditService = (IDisposable)userCreditServiceClientFactory.UserCreditService)
                {
                    var creditLimit = ((IUserCreditService)userCreditService).GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                    creditLimit = creditLimit*2;
                    user.CreditLimit = creditLimit;
                }
            }
            else
            {
                // Do credit check
                user.HasCreditLimit = true;
                using (var userCreditService = (IDisposable)userCreditServiceClientFactory.UserCreditService)
                {
                    var creditLimit = ((IUserCreditService)userCreditService).GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                    user.CreditLimit = creditLimit;
                }
            }

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            userDataAccess.AddUser(user);
            return true;
        }
    }
}
