using Microsoft.AspNetCore.Identity;

public class User: IdentityUser
{
    public string Name { get; set; } = string.Empty;   
    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}
