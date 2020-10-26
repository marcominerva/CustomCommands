using Microsoft.AspNetCore.Authentication;

namespace DeviceControl.Authentication
{
    public class ApiKeyOptions : AuthenticationSchemeOptions
    {
        public string KeyName { get; set; }

        public string KeyValue { get; set; }
    }
}
