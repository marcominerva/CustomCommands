using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceControl.Authentications
{
    public static class ApiKeyExtensions
    {
        public const string ApiKeyScheme = "ApiKey";

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions)
        {
            return builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(ApiKeyScheme, displayName: null, configureOptions);
        }
    }
}
