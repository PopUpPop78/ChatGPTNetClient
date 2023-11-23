using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

string? secret;

while(true)
{
    Console.WriteLine("Enter your ChatGPT secret (or type exit to close)");
    secret = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(secret))
        Console.WriteLine("Secret must contain a value");
    else
    {
        if(secret.Equals("exit", StringComparison.OrdinalIgnoreCase))
            return;
        break;
    }
}
while(true)
{
    Console.WriteLine("Enter your question (or type exit to close)");
    var question = Console.ReadLine();
    
    if(question == "exit")
        return;

    if(!string.IsNullOrWhiteSpace(question))
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {secret}");
        
        var request = new Request{ Prompt = question };

        var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = System.Text.Json.JsonSerializer.Serialize(request, jsonOpts);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
        var responseString = await response.Content.ReadAsStringAsync();

        try
        {
            var resObject = JsonConvert.DeserializeObject<Response>(responseString);
            Console.WriteLine(resObject?.Choices?.FirstOrDefault()?.Text);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }
}

public class Response
{
    public IEnumerable<Choice>? Choices { get; set; }
}

public class Choice
{
    public string? Text { get; set; }
}

public class Request
{
    public string? Model { get; set; } = "gpt-3.5-turbo-instruct";
    public string? Prompt { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 3000;
    public double Temperature { get; set; } = 1;
}
