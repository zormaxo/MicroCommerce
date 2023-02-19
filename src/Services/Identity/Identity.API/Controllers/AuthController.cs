using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService identityService;

    public AuthController(IIdentityService identityService) { this.identityService = identityService; }


    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
    {
        var result = await identityService.Login(loginRequestModel);

        return Ok(result);
    }
}
