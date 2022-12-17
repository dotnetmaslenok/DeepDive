using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestCommunications.ReceiverMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HomeController : Controller
{
    [Authorize]
    [Route("[action]")]
    public string Index()
    {
        return "String from receiver microservice";
    }
}