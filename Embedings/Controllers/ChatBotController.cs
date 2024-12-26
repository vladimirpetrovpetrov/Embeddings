using Embedings.Models;
using Embedings.Services;
using Microsoft.AspNetCore.Mvc;
using MSSqlServerDB;

namespace Embedings.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatBotController : ControllerBase
{
    private readonly EmbedingsDbContext _context;

    public ChatBotController(EmbedingsDbContext context)
    {
        _context = context;
    }

    [HttpPost("SaveChunk")]
    public async Task<IActionResult> SaveChunk([FromBody] UpsertVectorRequest request)
    {
        var embedding = await EmbeddingService.GetEmbeddingAsync(request.Input);
        var metadata = new Pinecone.Metadata();
        foreach (var kvp in request.Metadata)
        {
            metadata[kvp.Key] = kvp.Value;
        }
        var result = await PineconeService.UpsertVectorAsync(embedding, metadata);
        return Ok(result);
    }


}
