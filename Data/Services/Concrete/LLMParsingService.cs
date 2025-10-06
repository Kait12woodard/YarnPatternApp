using System.Text;
using System.Text.Json;
using YarnPatternApp.Models.ViewModels;
using YarnPatternApp.Data.Services.Abstract;
using PDFtoImage;
using SkiaSharp;

namespace YarnPatternApp.Data.Services.Concrete
{
    public class LLMParsingService : ILLMParsingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LLMParsingService> _logger;
        private readonly string _model = "llava:7b";

        public LLMParsingService(HttpClient httpClient, ILogger<LLMParsingService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://192.168.1.5:11434");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
            _logger = logger;
        }

        public async Task<NewPattern> ReviewAndEnhancePatternAsync(string pdfPath, NewPattern parsedPattern)
        {
            try
            {
                var images = ConvertPdfToImages(pdfPath, maxPages: 10);

                if (!images.Any())
                {
                    _logger.LogWarning("No images extracted from PDF");
                    return parsedPattern;
                }

                var prompt = BuildPrompt(parsedPattern);

                var requestBody = new
                {
                    model = _model,
                    prompt = prompt,
                    images = images,
                    stream = false
                };

                _logger.LogInformation("Sending pattern to LLM for review");

                var response = await _httpClient.PostAsync("/api/generate",
                    new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("LLM returned status {Status}", response.StatusCode);
                    return parsedPattern;
                }

                var result = await response.Content.ReadAsStringAsync();
                var llmResponse = JsonSerializer.Deserialize<LLMResponse>(result);

                return MergeWithLLMResults(parsedPattern, llmResponse.response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling LLM");
                return parsedPattern;
            }
        }

        private string BuildPrompt(NewPattern pattern)
        {
            return $@"You are reviewing a crochet/knitting pattern. Look at the images carefully. ONLY add or correct information that you can CLEARLY SEE in the images. DO NOT guess or make up information.

Current extracted data:
Name: {pattern.Name ?? "NOT FOUND"}
Designer: {pattern.Designer ?? "NOT FOUND"}
Craft Type: {pattern.CraftType ?? "NOT FOUND"}
Difficulty: {pattern.Difficulty?.ToString() ?? "NOT FOUND"}
Source URL: {pattern.PatSource ?? "NOT FOUND"}
Yarn Weights: {(pattern.YarnWeights?.Any() == true ? string.Join(", ", pattern.YarnWeights) : "NOT FOUND")}
Tool Sizes: {(pattern.ToolSizes?.Any() == true ? string.Join(", ", pattern.ToolSizes) : "NOT FOUND")}
Project Types: {(pattern.ProjectTypes?.Any() == true ? string.Join(", ", pattern.ProjectTypes) : "NOT FOUND")}
Yarn Brands: {(pattern.YarnBrands?.Any() == true ? string.Join(", ", pattern.YarnBrands) : "NOT FOUND")}
Tags: {(pattern.Tags?.Any() == true ? string.Join(", ", pattern.Tags) : "NOT FOUND")}

CRITICAL: Only include yarn brands you can actually read in the images. If you cannot see any brand names, use an empty array [].

Respond ONLY with valid JSON:
{{
    ""name"": ""BUTTERFLY TOP"",
    ""designer"": ""Mae Crochets"",
    ""craftType"": ""Crochet"",
    ""difficulty"": 2,
    ""patSource"": ""https://example.com"",
    ""yarnWeights"": [""4""],
    ""toolSizes"": [""5.0""],
    ""projectTypes"": [""Top""],
    ""yarnBrands"": [""Lion Brand"", ""Red Heart""],
    ""tags"": [""seamless"", ""lace""]
}}

If you cannot see information, use null or []. No explanations.";
        }

        private List<string> ConvertPdfToImages(string pdfPath, int maxPages = 10)
        {
            var base64Images = new List<string>();

            try
            {
                using var pdfStream = File.OpenRead(pdfPath);

                for (int i = 0; i < maxPages; i++)
                {
                    try
                    {
                        var skBitmap = Conversion.ToImage(pdfStream, page: i);
                        using var data = skBitmap.Encode(SKEncodedImageFormat.Jpeg, 80);
                        var base64 = Convert.ToBase64String(data.ToArray());
                        base64Images.Add(base64);

                        pdfStream.Position = 0;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting PDF to images");
            }

            return base64Images;
        }

        private NewPattern MergeWithLLMResults(NewPattern original, string llmResponse)
        {
            try
            {
                var jsonStart = llmResponse.IndexOf('{');
                var jsonEnd = llmResponse.LastIndexOf('}');

                if (jsonStart == -1 || jsonEnd == -1)
                {
                    _logger.LogWarning("No JSON found in LLM response");
                    return original;
                }

                var jsonStr = llmResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var enhanced = JsonSerializer.Deserialize<LLMPatternData>(jsonStr, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (enhanced == null) return original;

                if (!string.IsNullOrEmpty(enhanced.Name) && string.IsNullOrEmpty(original.Name))
                    original.Name = enhanced.Name;

                if (!string.IsNullOrEmpty(enhanced.Designer) && string.IsNullOrEmpty(original.Designer))
                    original.Designer = enhanced.Designer;

                if (!string.IsNullOrEmpty(enhanced.CraftType) && string.IsNullOrEmpty(original.CraftType))
                    original.CraftType = enhanced.CraftType;

                if (enhanced.Difficulty.HasValue && !original.Difficulty.HasValue)
                    original.Difficulty = enhanced.Difficulty;

                if (!string.IsNullOrEmpty(enhanced.PatSource) && string.IsNullOrEmpty(original.PatSource))
                    original.PatSource = enhanced.PatSource;

                if (enhanced.YarnWeights?.Any() == true)
                {
                    var combined = new List<string>(original.YarnWeights ?? new List<string>());
                    foreach (var weight in enhanced.YarnWeights)
                    {
                        if (!combined.Contains(weight))
                            combined.Add(weight);
                    }
                    original.YarnWeights = combined.Distinct().ToList();
                }

                if (enhanced.ToolSizes?.Any() == true)
                {
                    var combined = new List<string>(original.ToolSizes ?? new List<string>());
                    foreach (var size in enhanced.ToolSizes)
                    {
                        if (!combined.Contains(size))
                            combined.Add(size);
                    }
                    original.ToolSizes = combined.Distinct().ToList();
                }

                if (enhanced.ProjectTypes?.Any() == true)
                {
                    var combined = new List<string>(original.ProjectTypes ?? new List<string>());
                    foreach (var type in enhanced.ProjectTypes)
                    {
                        if (!combined.Contains(type))
                            combined.Add(type);
                    }
                    original.ProjectTypes = combined.Distinct().ToList();
                }

                if (enhanced.YarnBrands?.Any() == true)
                {
                    var combined = new List<string>(original.YarnBrands ?? new List<string>());
                    foreach (var brand in enhanced.YarnBrands)
                    {
                        if (!combined.Contains(brand))
                            combined.Add(brand);
                    }
                    original.YarnBrands = combined.Distinct().ToList();
                }

                if (enhanced.Tags?.Any() == true)
                {
                    var combined = new List<string>(original.Tags ?? new List<string>());
                    foreach (var tag in enhanced.Tags)
                    {
                        if (!combined.Contains(tag))
                            combined.Add(tag);
                    }
                    original.Tags = combined.Distinct().ToList();
                }

                _logger.LogInformation("Successfully enhanced pattern with LLM data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse LLM response: {Response}", llmResponse);
            }

            return original;
        }

        private class LLMResponse
        {
            public string response { get; set; }
        }

        private class LLMPatternData
        {
            public string Name { get; set; }
            public string Designer { get; set; }
            public string CraftType { get; set; }
            public int? Difficulty { get; set; }
            public string PatSource { get; set; }
            public List<string> YarnWeights { get; set; }
            public List<string> ToolSizes { get; set; }
            public List<string> ProjectTypes { get; set; }
            public List<string> YarnBrands { get; set; }
            public List<string> Tags { get; set; }
        }
    }
}