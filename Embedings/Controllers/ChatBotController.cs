using Embedings.Interfaces;
using Embedings.Models;
using Microsoft.AspNetCore.Mvc;

namespace Embedings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBotService _chatBotService;

        public ChatBotController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        [HttpPost("ProcessText")]
        public async Task<IActionResult> ProcessText([FromBody] ProcessTextRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Input))
            {
                return BadRequest("Input text cannot be empty.");
            }

            var result = await _chatBotService.ProcessTextAsync(request.Input, request.Metadata);
            return Ok(result);
        }
    }
}
