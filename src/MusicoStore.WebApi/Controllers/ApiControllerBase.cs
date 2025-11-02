using Microsoft.AspNetCore.Mvc;

namespace MusicoStore.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
}
