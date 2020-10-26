using Microsoft.AspNetCore.Authentication;
using System;

namespace DeviceControl.Authentication
{
    public static class ApiKeyExtensions
    {
        public const string ApiKeyScheme = "ApiKey";

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions)
            => builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(ApiKeyScheme, displayName: null, configureOptions);
    }
}
