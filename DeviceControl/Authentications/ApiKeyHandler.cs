using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DeviceControl.Authentications
{
    internal class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
    {
        public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
            if (!this.Context.Request.Query.ContainsKey(this.Options.KeyName) && !this.Context.Request.Headers.ContainsKey(this.Options.KeyName))
            {
                return AuthenticateResult.NoResult();
            }

            if (this.Context.Request.Query[this.Options.KeyName] == this.Options.KeyValue || this.Context.Request.Headers[this.Options.KeyName] == this.Options.KeyValue)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "ApiUser")
                };

                var scheme = this.Scheme.Name;
                var identity = new ClaimsIdentity(claims, scheme);
                var principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Invalid key");
        }
	}
}
