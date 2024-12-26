﻿using Embedings.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace Embedings.Services
{
    public class GPTService : IGPTService
    {
        private readonly string _huggingFaceApiKey;
        private readonly string _gptNeoUrl;
        private readonly string _gpt2Url;
        private readonly string _mixtralModelUrl;

        public GPTService(IConfiguration configuration)
        {
            _huggingFaceApiKey = configuration["GPTService:HuggingFaceApiKey"];
            _gptNeoUrl = configuration["GPTService:GptNeoUrl"];
            _gpt2Url = configuration["GPTService:Gpt2Url"];
            _mixtralModelUrl = configuration["GPTService:MixtralModelUrl"];
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_huggingFaceApiKey}");

            var prompt2 = @"You are an AI assistant helping a user with information about our API. 
            Below are three sentences retrieved from the documentation that are closely related to the user's question. 
            Please rewrite the information into a single, concise, and coherent sentence that answers the question clearly.

            Sentence 1: The authentication process requires a valid API key to be included in the `Authorization` header of each request.
            Sentence 2: To obtain an API key, users must register an account and generate the key from the dashboard.
            Sentence 3: Requests without a valid API key will return a 401 Unauthorized error.

            [Task:] Combine the above sentences into a single, well-structured sentence.";

            var instructionTemplate = $"<s> [INST] {prompt2} [/INST]";

            var requestData = new
            {
                inputs = instructionTemplate,
                parameters = new
                {
                    max_length = 100,
                    temperature = 0.7
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_mixtralModelUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error: {response.StatusCode}, {errorContent}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseData);

            return result[0]?.generated_text ?? "No response generated.";
        }

        public async Task<string> GetResponseAsync2(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_huggingFaceApiKey}");

            var instructionTemplate = $"<s> [INST] {prompt} [/INST]";

            var requestData = new
            {
                inputs = instructionTemplate,
                parameters = new
                {
                    max_length = 100,
                    temperature = 0.7
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_mixtralModelUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error: {response.StatusCode}, {errorContent}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseData);

            return result[0]?.generated_text ?? "No response generated.";
        }

        public async Task<string> GetResponseAsync3(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_huggingFaceApiKey}");

            var requestData = new
            {
                inputs = prompt
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_gptNeoUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error Response: {errorContent}");
                throw new Exception($"Error: {response.StatusCode}, {errorContent}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseData);

            return result[0]?.generated_text ?? "No response generated.";
        }
    }
}
