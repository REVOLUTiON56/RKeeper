using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RKeeper.Api.Controllers;

[ApiController]
[Route("v1/test")]
[EnableRateLimiting("default")]
public class TestController : ControllerBase
{
    public TestController()
    {
        
    }

    [HttpGet]
    public async Task<IActionResult> Test() {
        return Ok("Test");
    }
}
