using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Demo.Authentication
{
    public class CustomTicketStore : ITicketStore
    {
        public Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            throw new NotImplementedException();
        }
    }
}