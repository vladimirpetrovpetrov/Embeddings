using Embedings.Interfaces;
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
    private readonly IEmbeddingService _embeddingService;
    private readonly IPineconeService _pineconeService;

    public ChatBotController(EmbedingsDbContext context,IEmbeddingService embeddingService,IPineconeService pineconeService)
    {
        _context = context;
        _embeddingService = embeddingService;
        _pineconeService = pineconeService;

    }

    [HttpPost("SaveChunk")]
    public async Task<IActionResult> SaveChunk([FromBody] UpsertVectorRequest request)
    {
        var embedding = await _embeddingService.GetEmbeddingAsync(request.Input);
        var metadata = new Pinecone.Metadata();
        foreach (var kvp in request.Metadata)
        {
            metadata[kvp.Key] = kvp.Value;
        }
        var result = await _pineconeService.UpsertVectorAsync(embedding, metadata);
        return Ok(result);
    }


}
