using Embedings.Models;
using Embedings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Embedings.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GPTController : ControllerBase
{
    [HttpPost("TestGPT")]
    public async Task<IActionResult> TestGPT([FromBody] GPTRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Inputs))
        {
            return BadRequest("Invalid request.");
        }

        var result = await GPTService.GetResponseAsync(request.Inputs);
        return Ok(result);
    }
}
