using System.Text.Json.Serialization;

public class Movie
{
    public int MovieId { get; set; } 
    public Movie()
    {
        MovieId = Guid.NewGuid().GetHashCode();
    }

    [JsonPropertyName("Movie title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("Genre")]
    public string Genre { get; set; } = string.Empty;

    [JsonPropertyName("Rating")]
    public double Rating { get; set; } = 0.0;

    [JsonPropertyName("Image URL")]
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}