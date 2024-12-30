using MovieGPT.Models;

public class Recommendation
{
    public int RecommendationId { get; set; } 
    public string UserId { get; set; }
    public int MovieId { get; set; }
    public Recommendation()
    {
        RecommendationId = new Guid().GetHashCode();
        UserId = User.Id;
        MovieId = Movie.MovieId;
    }
    public string SuggestedBy { get; set; } = string.Empty; 
    public DateTime DateSuggested { get; set; } // Date when the recommendation was made
    
    public User User { get; set; } = new User();
    public Movie Movie { get; set; } = new Movie();
    
}