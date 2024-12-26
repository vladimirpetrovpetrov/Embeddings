using Embedings.Interfaces;
using Embedings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Embedings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbeddingsController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;

        public EmbeddingsController(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        [HttpPost("Vectorize")]
        public async Task<IActionResult> Vectorize([FromQuery] string input)
        {
            var vector = await _embeddingService.GetEmbeddingAsync(input);
            return Ok(vector);
        }

        [HttpPost("euclidean")]
        public async Task<IActionResult> Euclidean([FromQuery] string word1, string word2)
        {
            var vec1 = await _embeddingService.GetEmbeddingAsync(word1);
            var vec2 = await _embeddingService.GetEmbeddingAsync(word2);

            var distance = _embeddingService.EuclideanDistance(vec1, vec2);
            return Ok(distance);
        }

        [HttpPost("cosine")]
        public async Task<IActionResult> Cosine([FromQuery] string word1, string word2)
        {
            var vec1 = await _embeddingService.GetEmbeddingAsync(word1);
            var vec2 = await _embeddingService.GetEmbeddingAsync(word2);

            var similarity = _embeddingService.CosineSimilarity(vec1, vec2);
            return Ok(similarity);
        }

        [HttpPost("manhattan")]
        public async Task<IActionResult> Manhattan([FromQuery] string word1, string word2)
        {
            var vec1 = await _embeddingService.GetEmbeddingAsync(word1);
            var vec2 = await _embeddingService.GetEmbeddingAsync(word2);

            var distance = _embeddingService.ManhattanDistance(vec1, vec2);
            return Ok(distance);
        }

        [HttpPost("SimilarityScore")]
        public async Task<IActionResult> Compare([FromQuery] string word1, string word2, string word3)
        {
            var similarityResult = await _embeddingService.CompareWordsAsync(word1, word2, word3);

            var results = new Dictionary<string, float>
            {
                { $"{word1} - {word2}", similarityResult.Score1 },
                { $"{word1} - {word3}", similarityResult.Score2 }
            };

            return Ok(results);
        }
    }
}
