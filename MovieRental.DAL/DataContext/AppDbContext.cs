using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.DAL.DataContext
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public required DbSet<Language> Languages { get; set; }
        public required DbSet<Currency> Currencies { get; set; }
        public required DbSet<Logo> Logos { get; set; }
        public required DbSet<Slider> Sliders { get; set; }
        public required DbSet<SliderTranslation> SliderTranslations { get; set; }
        public required DbSet<Movie> Movies { get; set; }
        public required DbSet<MovieTranslation> MovieTranslations { get; set; }
        public required DbSet<MovieGenre> MovieGenres { get; set; }
        public required DbSet<MovieActor> MovieActors { get; set; }
        public required DbSet<MovieImage> MovieImages { get; set; }
        public required DbSet<MovieVideo> MovieVideos { get; set; }
        public required DbSet<MovieVideoTranslation> MovieVideoTranslations { get; set; }
        public required DbSet<MovieSocialLink> MovieSocialLinks { get; set; }
        public required DbSet<MovieTab> MovieTabs { get; set; }
        public required DbSet<MovieTabTranslation> MovieTabTranslations { get; set; }
        public required DbSet<Genre> Genres { get; set; }
        public required DbSet<GenreTranslation> GenreTranslations { get; set; }
        public required DbSet<Actor> Actors { get; set; }
        public required DbSet<ActorTranslation> ActorTranslations { get; set; }
        public required DbSet<Collection> Collections { get; set; }
        public required DbSet<CollectionMovie> CollectionMovies { get; set; }
        public required DbSet<CollectionTranslation> CollectionTranslations { get; set; }
        public required DbSet<Event> Events { get; set; }
        public required DbSet<EventTranslation> EventTranslations { get; set; }
        public required DbSet<Sport> Sports { get; set; }
        public required DbSet<SportTranslation> SportTranslations { get; set; }
        public required DbSet<Review> Reviews { get; set; }
        public required DbSet<ReviewReaction> ReviewReactions { get; set; }
        public required DbSet<ShareLog> ShareLogs { get; set; }
        public required DbSet<UserWatchlist> UserWatchlists { get; set; }
        public required DbSet<Rental> Rentals { get; set; }
        public required DbSet<Offer> Offers { get; set; }
        public required DbSet<OfferTranslation> OfferTranslations { get; set; }
        public required DbSet<Notification> Notifications { get; set; }
        public required DbSet<NotificationTranslation> NotificationTranslations { get; set; }
        public required DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieTranslation>()
                .HasOne(mt => mt.Movie)
                .WithMany(m => m.MovieTranslations)
                .HasForeignKey(mt => mt.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieTranslation>()
                .HasOne(mt => mt.Language)
                .WithMany()
                .HasForeignKey(mt => mt.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany()
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany()
                .HasForeignKey(mg => mg.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieVideoTranslation>()
                .HasOne(mvt => mvt.MovieVideo)
                .WithMany(mv => mv.MovieVideoTranslations)
                .HasForeignKey(mvt => mvt.MovieVideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieVideoTranslation>()
                .HasOne(mvt => mvt.Language)
                .WithMany()
                .HasForeignKey(mvt => mvt.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieTabTranslation>()
                .HasOne(mtt => mtt.MovieTab)
                .WithMany(mt => mt.MovieTabTranslations)
                .HasForeignKey(mtt => mtt.MovieTabId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieTabTranslation>()
                .HasOne(mtt => mtt.Language)
                .WithMany()
                .HasForeignKey(mtt => mtt.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review configuration
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ParentReview)
                .WithMany(r => r.Replies)
                .HasForeignKey(r => r.ParentReviewId)
                .OnDelete(DeleteBehavior.Restrict); 

            // ReviewReaction configuration
            modelBuilder.Entity<ReviewReaction>()
                .HasOne(rr => rr.Review)
                .WithMany()
                .HasForeignKey(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<ReviewReaction>()
                .HasOne(rr => rr.User)
                .WithMany()
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Restrict);  
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is TimeStample &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var timestamp = (TimeStample)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    timestamp.CreatedAt = DateTime.UtcNow;
                }

                timestamp.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
