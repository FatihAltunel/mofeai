using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieGPT.ApplicationServices
{
    
public class OpenAiService
{
    
    protected internal HttpClient _httpClient;

    public OpenAiService(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
        {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = prompt }
        },
            max_tokens = 100,
            temperature = 0.7
        };

        var jsonRequestBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<JsonDocument>(responseContent);
            return responseObject?.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No response.";
        }
        else
        {
            return $"Error: {response.StatusCode}";
        }
    }

    public async Task<string> GetResult(string prompt)
    {
        string _content =
        $"Based on the movie '{prompt}', suggest 1 similar movies with the following details for each: " +
        "- Movie title, Genre, Rating (out of 10), and Image URL. " +
        "Ensure the response is in valid JSON format. " +
        "The object should have the keys: 'Movie title', 'Genre', 'Rating', and 'Image URL'. " +
        "The rating should be a number between 0 and 10, without the '/10' notation. " +
        "The image URLs should be sourced from Wikipedia and must not be long URLs."+
        "Do NOT use "+"json word"+"in the response"+
        "Do not include any commentary, just return the movie suggestions in the required format.";
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
        {
            new { role = "system", content = _content },
            new { role = "user", content = prompt }
        },
            max_tokens = 100,
            temperature = 0.7
        };

        var jsonRequestBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<JsonDocument>(responseContent);
            var jsonResult = responseObject?.RootElement.GetProperty("choices")[0]
                                        .GetProperty("message")
                                        .GetProperty("content")
                                        .GetString();

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                try
                {
                    Console.WriteLine(jsonResult);
                    var movie = JsonSerializer.Deserialize<Movie>(jsonResult);
                    
                    return JsonSerializer.Serialize(movie ?? new Movie());
                }
                catch (JsonException)
                {
                    return JsonSerializer.Serialize(new Movie()); 
                }
            }
        }

        return JsonSerializer.Serialize(new Movie()); 
    }   

    public async Task<string> GetMoreThanOneResult(string prompt){

    string _content = 
        $"Based on the movie '{prompt}', suggest 10 similar movies with the following details for each: " +
        "- Movie title, Genre, Rating (out of 10), and Image URL. " +
        "Ensure the response is in valid JSON format as an array of objects. " +
        "Each object should have the keys: 'Movie title', 'Genre', 'Rating', and 'Image URL'. " +
        "The rating should be a number between 0 and 10, without the '/10' notation. " +
        "The image URLs should be sourced from Wikipedia and must not be long URLs."+
        "Do not include any commentary, just return the movie suggestions in the required format.";

    var requestBody = new
    {
        model = "gpt-4",
        messages = new[]
        {
            new { role = "system", content = _content },
            new { role = "user", content = prompt }
        },
        max_tokens = 1500,
        temperature = 0.7
    };

    var jsonRequestBody = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
    if (response.IsSuccessStatusCode)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JsonDocument>(responseContent);
        var jsonResult = responseObject?.RootElement.GetProperty("choices")[0]
                                      .GetProperty("message")
                                      .GetProperty("content")
                                      .GetString();

        if (!string.IsNullOrWhiteSpace(jsonResult))
        {
            try
            {
                Console.WriteLine(JsonSerializer.Deserialize<object>(jsonResult));

                var movies = JsonSerializer.Deserialize<List<Movie>>(jsonResult);
                
                return JsonSerializer.Serialize(movies ?? new List<Movie>());
            }
            catch (JsonException)
            {
                return JsonSerializer.Serialize(new List<Movie>()); // JSON hatası durumunda boş bir liste döndür
            }
        }
    }

    return JsonSerializer.Serialize(new List<Movie>()); // Başarısız API çağrısında boş bir liste döndür

    }
}
}