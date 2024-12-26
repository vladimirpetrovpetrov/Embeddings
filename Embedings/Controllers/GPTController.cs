using Embedings.Interfaces;
using Embedings.Models;
using Embedings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Embedings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly IGPTService _gptService;

        public GPTController(IGPTService gptService)
        {
            _gptService = gptService;
        }

        [HttpPost("TestGPT")]
        public async Task<IActionResult> TestGPT([FromBody] GPTRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Inputs))
            {
                return BadRequest("Invalid request.");
            }

            var result = await _gptService.GetResponseAsync(request.Inputs);
            return Ok(result);
        }
    }
}
