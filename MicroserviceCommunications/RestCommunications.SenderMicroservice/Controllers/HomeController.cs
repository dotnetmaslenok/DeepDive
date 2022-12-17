using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace RestCommunications.SenderMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HomeController : Controller
{
	private readonly IHttpClientFactory _httpClientFactory;

	public HomeController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	[Route("[action]")]
	public async Task<IActionResult> Index()
	{
		var authClient = _httpClientFactory.CreateClient();

		var discoveryDocument = await authClient.GetDiscoveryDocumentAsync("https://localhost:7181");
		var tokenResponse = await authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
		{
			Address = discoveryDocument.TokenEndpoint,

			ClientId = "senderMicroservice",
			ClientSecret = "sender-microservice-client-secret",

			Scope = "receiverMicroservice",
		});

		var client = _httpClientFactory.CreateClient();

		client.SetBearerToken(tokenResponse.AccessToken);

		var response = await client.GetAsync("https://localhost:7276/home/index");

		if (!response.IsSuccessStatusCode)
		{
			throw new BadHttpRequestException(
				$"Bad http request on uri: https://localhost:7276/home/index{Environment.NewLine}StatusCode: {response.StatusCode}");
		}

		var content = await response.Content.ReadAsStringAsync();

		return Ok(content);
	}
}