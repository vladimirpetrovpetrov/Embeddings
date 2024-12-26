using Embedings.Models;
using Microsoft.AspNetCore.Mvc;
using Pinecone;

namespace Embedings.Services;

public static class PineconeService
{
    private static string? _apiKey;
    private static string? _indexName;
    private static string? _host;

    public static void Initialize(IConfiguration configuration)
    {
        _apiKey = configuration["PineconeService:ApiKey"];
        _indexName = configuration["PineconeService:IndexName"];
        _host = configuration["PineconeService:Host"];
    }


    public static async Task<string> UpsertVectorAsync(float[] embedding, Metadata metadata)
    {
        var indexClient = await GetIndexClientAsync();

        var vectors = new List<Pinecone.Vector>
        {
            new Pinecone.Vector
            {
                Id = Guid.NewGuid().ToString(),
                Values = new ReadOnlyMemory<float>(embedding),
                Metadata = metadata
            }
        };

        var upsertRequest = new Pinecone.UpsertRequest
        {
            Vectors = vectors
        };

        var grpcOptions = new GrpcRequestOptions
        {
            MaxRetries = 3,
            Timeout = TimeSpan.FromSeconds(10)
        };

        var response = await indexClient.UpsertAsync(upsertRequest, grpcOptions);
        return response.UpsertedCount.ToString();
    }


    public async static Task<Pinecone.Vector?> FetchVector(string id)
    {
        var indexClient = await GetIndexClientAsync();

        var response = await indexClient.FetchAsync(new Pinecone.FetchRequest { Ids = new List<string> { id } });

        return response.Vectors.FirstOrDefault().Value;
    }


    public static async Task<Pinecone.QueryResponse> QueryVectors(QueryRequestData data, float[] embedding)
    {
        var indexClient = await GetIndexClientAsync();

        var queryRequest = new Pinecone.QueryRequest
        {
            TopK = data.TopK,
            IncludeValues = data.IncludeValues,
            IncludeMetadata = data.IncludeMetadata,
            Vector = embedding
        };

        var response = await indexClient.QueryAsync(queryRequest);

        return response;
    }

    public static async Task<string> UpdateVector([FromBody] UpdateRequestData data)
    {
        var indexClient = await GetIndexClientAsync();

        Metadata? metadata = null;
        if (data.Metadata != null)
        {
            metadata = new Metadata();
            foreach (var kvp in data.Metadata)
            {
                metadata[kvp.Key] = kvp.Value;
            }
        }

        var updateRequest = new Pinecone.UpdateRequest
        {
            Id = data.Id,
            Values = data.Values != null ? new ReadOnlyMemory<float>(data.Values) : null,
            SetMetadata = metadata,
            Namespace = data.Namespace
        };

        await indexClient.UpdateAsync(updateRequest);

        return "";
    }

    public static async Task<string> DeleteVector(string id)
    {
        var indexClient = await GetIndexClientAsync();

        var deleteRequest = new Pinecone.DeleteRequest
        {
            Ids = new List<string> { id }
        };

        await indexClient.DeleteAsync(deleteRequest);

        return "";
    }
    public static async Task<List<IndexData>> ListIndexes()
    {
        var client = CreateClient();

        var indexes = await client.ListIndexesAsync();
        var result = new List<IndexData>();
        foreach (var item in indexes.Indexes)
        {
            var index = new IndexData
            {
                Dimension = item.Dimension,
                Name = item.Name,
            };
            result.Add(index);
        };
        return result;
    }

    internal static async Task<Pinecone.DescribeIndexStatsResponse> IndexStats(string indexName)
    {
        var indexClient = await GetIndexClientAsync();
        var example = new Pinecone.DescribeIndexStatsRequest();
        var stats = await indexClient.DescribeIndexStatsAsync(example);

        return stats;
    }

    private static PineconeClient CreateClient()
    {
        return new PineconeClient(apiKey);
    }

    private static async Task<IndexClient> GetIndexClientAsync()
    {
        var client = CreateClient();
        return client.Index(name: indexName, host: host);
    }

    public static async Task<string> UpsertVectorAsync(string input, float[] embedding, Metadata metadata)
    {
        var indexClient = await GetIndexClientAsync();

        var vectors = new List<Pinecone.Vector>
        {
            new Pinecone.Vector
            {
                Id = Guid.NewGuid().ToString(),
                Values = new ReadOnlyMemory<float>(embedding),
                Metadata = metadata
            }
        };

        var upsertRequest = new Pinecone.UpsertRequest
        {
            Vectors = vectors
        };

        var grpcOptions = new GrpcRequestOptions
        {
            MaxRetries = 3,
            Timeout = TimeSpan.FromSeconds(10)
        };

        var response = await indexClient.UpsertAsync(upsertRequest, grpcOptions);

        if (response.UpsertedCount > 0) { 
        
        }

        return response.UpsertedCount.ToString();
    }
}
