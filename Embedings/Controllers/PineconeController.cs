using Embedings.Models;
using Embedings.Services;
using Microsoft.AspNetCore.Mvc;
using MSSqlServerDB;

namespace Embedings.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PineconeController : ControllerBase
{
    private readonly EmbedingsDbContext _context;

    public PineconeController(EmbedingsDbContext context)
    {
        _context = context;
    }

    [HttpPost("UpsertVector")]
    public async Task<IActionResult> UpsertVector([FromBody] UpsertVectorRequest request)
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

    [HttpGet("fetch/{id}")]
    public async Task<IActionResult> FetchVector(string id)
    {
        var fetchedVector = await PineconeService.FetchVector(id);

        var metadata = fetchedVector.Metadata.FirstOrDefault();

        return Ok(metadata.ToString());
    }

    [HttpPost("query")]
    public async Task<IActionResult> QueryVectors([FromBody] QueryRequestData data)
    {
        var embedding = await EmbeddingService.GetEmbeddingAsync(data.SearchInput);
        var response = await PineconeService.QueryVectors(data, embedding);

        var matches = response.Matches.FirstOrDefault();
        var result = new
        {
            Id = matches.Id,
            Score = matches.Score,
            MetaDataInfo = matches.Metadata.FirstOrDefault()
        };

        return Ok(result.MetaDataInfo.Value.Value);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateVector([FromBody] UpdateRequestData data)
    {
        var response = await PineconeService.UpdateVector(data);

        return Ok();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteVector(string id, [FromQuery] string? namespaceName = null)
    {
        await PineconeService.DeleteVector(id);

        return Ok();
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListIndexes()
    {
        var indexes = await PineconeService.ListIndexes();

        return Ok(indexes);
    }

    [HttpGet("index-stats/{indexName}")]
    public async Task<IActionResult> GetIndexStats(string indexName)
    {
        var stats = await PineconeService.IndexStats(indexName);
        var info = $"Index: {indexName} = Total Vector Count: {stats.TotalVectorCount}, Dimensions: {stats.Dimension}, Fullness: {stats.IndexFullness}";
        return Ok(info);
    }

    [HttpGet("Upsert-Documentation")]
    public async Task<IActionResult> UpsertDocumentation(string indexName)
    {



        return Ok();
    }

}