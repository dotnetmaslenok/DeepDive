using Microsoft.AspNetCore.Identity;

namespace RestCommunications.IdentityServer.Identity
{
	internal class ApplicationUser : IdentityUser<Guid>
	{
		public string DisplayName { get; set; }
	}
}
