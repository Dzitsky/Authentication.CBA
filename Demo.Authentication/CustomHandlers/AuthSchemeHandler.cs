using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Demo.Authentication.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Authentication.CustomHandlers
{
    /// <summary>
    /// Простой пример обработчика аутентификации.
    /// В обработчике можно решить, можем ли мы аутентифицировать пользователя или нет.
    /// Логика проверки может быть произвольной, в зависимости от задачи
    /// </summary>
    public class AuthSchemeHandler : AuthenticationHandler<AuthSchemeOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Random _random;

        public AuthSchemeHandler(
            IOptionsMonitor<AuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _random = new Random();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_random.Next(0, 100) % 2 == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Не повезло"));
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "Пользователь")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
