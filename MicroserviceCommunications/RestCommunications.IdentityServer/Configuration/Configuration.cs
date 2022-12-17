using IdentityServer4;
using IdentityServer4.Models;

namespace RestCommunications.IdentityServer.Configuration
{
	internal static class Configuration
	{
		public static IEnumerable<Client> GetClients()
		{
			return new List<Client>()
			{
				new Client()
				{
					ClientName = "senderMicroservice",
					ClientId = "senderMicroservice",
					ClientSecrets = { new Secret("sender-microservice-client-secret".Sha256()) },
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					RequirePkce = true,
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"receiverMicroservice",
					},
					RequireClientSecret = false,
					RequireConsent = false,
					AccessTokenLifetime = 120,
				}
			};
		}

		public static IEnumerable<ApiScope> GetApiScopes()
		{
			return new List<ApiScope>()
			{
				new ApiScope("receiverMicroservice", "Sender microservice scope")
			};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>()
			{
				new ApiResource("receiverMicroservice", "Sender microservice scope")
				{
					Scopes = { "receiver" }
				}
			};
		}

		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>()
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
			};
		}
	}
}
