using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DeviceControl.Authentication
{
    internal class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
    {
        public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticateResult result;

            var request = Context.Request;
            if (!request.Query.ContainsKey(Options.KeyName) && !request.Headers.ContainsKey(Options.KeyName))
            {
                result = AuthenticateResult.NoResult();
            }
            else
            {
                if (request.Query[Options.KeyName] == Options.KeyValue || request.Headers[Options.KeyName] == Options.KeyValue)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "ApiUser")
                    };

                    var scheme = Scheme.Name;

                    var identity = new ClaimsIdentity(claims, scheme);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, scheme);

                    result = AuthenticateResult.Success(ticket);
                }
                else
                {
                    result = AuthenticateResult.Fail("Invalid key");
                }
            }

            return Task.FromResult(result);
        }
    }
}
