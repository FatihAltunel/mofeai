using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
     public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    public DbSet<User>? User { get; set; }
    public DbSet<Movie>? Movies { get; set; }
    public DbSet<Recommendation>? Recommendations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserLogin<string>>()
        .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        modelBuilder.Entity<IdentityUserRole<string>>()
        .HasKey(r => new { r.UserId, r.RoleId });

        modelBuilder.Entity<IdentityUserToken<string>>()
        .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Recommendations)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);

        // Movie Configuration
        modelBuilder.Entity<Movie>()
            .HasKey(m => m.MovieId);

        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Recommendations)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId);

        // Recommendation Configuration
        modelBuilder.Entity<Recommendation>()
            .HasKey(r => r.RecommendationId);

        modelBuilder.Entity<Recommendation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Recommendations)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Recommendation>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Recommendations)
            .HasForeignKey(r => r.MovieId);
}
}