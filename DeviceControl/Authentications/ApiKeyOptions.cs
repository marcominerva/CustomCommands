using Microsoft.AspNetCore.Authentication;

namespace DeviceControl.Authentications
{
	public class ApiKeyOptions : AuthenticationSchemeOptions
	{
		public string KeyName { get; set; }

		public string KeyValue { get; set; }
	}
}
