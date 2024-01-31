using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Demo.Authentication
{
    public class CustomTicketStore : ITicketStore
    {
        private IMemoryCache _cache;

        public CustomTicketStore(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
            
        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new MemoryCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }
            options.SetSlidingExpiration(TimeSpan.FromDays(100*356));//on server
            _cache.Set(key, ticket, options);
            return Task.FromResult(0);        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            AuthenticationTicket ticket;
            _cache.TryGetValue(key, out ticket);
            return Task.FromResult(ticket);        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid();
            var key = DateTime.Now + guid.ToString();
            await RenewAsync(key, ticket);
            return key;
        }
    }
}