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
        public required DbSet<MoviePerson> MoviePersons { get; set; }
        public required DbSet<MovieImage> MovieImages { get; set; }
        public required DbSet<MovieVideo> MovieVideos { get; set; }
        public required DbSet<MovieVideoTranslation> MovieVideoTranslations { get; set; }
        public required DbSet<MovieSocialLink> MovieSocialLinks { get; set; }
        public required DbSet<Genre> Genres { get; set; }
        public required DbSet<GenreTranslation> GenreTranslations { get; set; }
        public required DbSet<Event> Events { get; set; }
        public required DbSet<EventTranslation> EventTranslations { get; set; }
        public required DbSet<Sport> Sports { get; set; }
        public required DbSet<SportTranslation> SportTranslations { get; set; }
        public required DbSet<Person> Persons { get; set; }
        public required DbSet<PersonTranslation> PersonTranslations { get; set; }
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

            #region Person Configuration

            modelBuilder.Entity<Person>()
                .HasMany(p => p.PersonTranslations)
                .WithOne(pt => pt.Person)
                .HasForeignKey(pt => pt.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersonTranslation>() 
                .HasOne(pt => pt.Language)
                .WithMany()
                .HasForeignKey(pt => pt.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonTranslation>()
                .HasIndex(pt => new { pt.PersonId, pt.LanguageId })
                .IsUnique();

            #endregion

            #region Movie Configuration

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.PosterImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(m => m.CoverImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(m => m.VideoUrl).IsRequired().HasMaxLength(500);
                entity.Property(m => m.TrailerUrl).HasMaxLength(500);
                entity.Property(m => m.Format).HasMaxLength(10);

                entity.Property(m => m.RentalPrice).HasColumnType("decimal(18,2)");
                entity.Property(m => m.Budget).HasColumnType("decimal(18,2)");

                entity.Property(m => m.LovePercentage).HasDefaultValue(0);
                entity.Property(m => m.VotesCount).HasDefaultValue(0);
                entity.Property(m => m.IsActive).HasDefaultValue(true);
                entity.Property(m => m.IsFeatured).HasDefaultValue(false);
                entity.Property(m => m.IsAvailableForRent).HasDefaultValue(true);
                entity.Property(m => m.RentalDurationDays).HasDefaultValue(30);

                entity.HasOne(m => m.Language)
                    .WithMany()
                    .HasForeignKey(m => m.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Currency)
                    .WithMany()
                    .HasForeignKey(m => m.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.Year);
                entity.HasIndex(m => m.ReleaseDate);
                entity.HasIndex(m => m.IsActive);
                entity.HasIndex(m => m.IsFeatured);
                entity.HasIndex(m => m.LanguageId);
                entity.HasIndex(m => m.IsDeleted); 

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<MovieTranslation>(entity =>
            {
                entity.HasKey(mt => mt.Id);

                entity.Property(mt => mt.Title).IsRequired().HasMaxLength(200);
                entity.Property(mt => mt.Plot).IsRequired().HasMaxLength(2000);
                entity.Property(mt => mt.Director).IsRequired().HasMaxLength(500);
                entity.Property(mt => mt.Writers).IsRequired().HasMaxLength(500);
                entity.Property(mt => mt.Cast).IsRequired().HasMaxLength(1000);

                entity.HasOne(mt => mt.Movie)
                    .WithMany(m => m.MovieTranslations)
                    .HasForeignKey(mt => mt.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mt => mt.Language)
                    .WithMany()
                    .HasForeignKey(mt => mt.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(mt => new { mt.MovieId, mt.LanguageId }).IsUnique();

                entity.HasQueryFilter(mg => !mg.IsDeleted);
            });

            modelBuilder.Entity<MovieGenre>(entity =>
            {
                entity.HasKey(mg => mg.Id);

                entity.HasOne(mg => mg.Movie)
                    .WithMany(m => m.MovieGenres)
                    .HasForeignKey(mg => mg.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mg => mg.Genre)
                    .WithMany(g => g.MovieGenres)
                    .HasForeignKey(mg => mg.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(mg => new { mg.MovieId, mg.GenreId }).IsUnique();

                entity.HasQueryFilter(mg => !mg.IsDeleted);
            });

            modelBuilder.Entity<MoviePerson>(entity =>
            {
                entity.HasKey(mp => mp.Id);

                entity.Property(mp => mp.CharacterName).HasMaxLength(200);
                entity.Property(mp => mp.IsActive).HasDefaultValue(true);
                entity.Property(mp => mp.DisplayOrder).HasDefaultValue(0);

                entity.HasOne(mp => mp.Movie)
                    .WithMany(m => m.MoviePersons)
                    .HasForeignKey(mp => mp.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mp => mp.Person)
                    .WithMany()
                    .HasForeignKey(mp => mp.PersonId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(mp => new { mp.MovieId, mp.PersonId, mp.Role });
                entity.HasIndex(mp => mp.Role); 

                entity.HasQueryFilter(mp => !mp.IsDeleted);
            });

            modelBuilder.Entity<MovieImage>(entity =>
            {
                entity.HasKey(mi => mi.Id);

                entity.Property(mi => mi.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(mi => mi.IsPrimary).HasDefaultValue(false);
                entity.Property(mi => mi.IsActive).HasDefaultValue(true);
                entity.Property(mi => mi.DisplayOrder).HasDefaultValue(0);

                entity.HasOne(mi => mi.Movie)
                    .WithMany(m => m.MovieImages)
                    .HasForeignKey(mi => mi.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(mi => mi.MovieId);
                entity.HasIndex(mi => new { mi.MovieId, mi.IsPrimary }); 

                entity.HasQueryFilter(mi => !mi.IsDeleted);
            });

            modelBuilder.Entity<MovieVideo>(entity =>
            {
                entity.HasKey(mv => mv.Id);

                entity.Property(mv => mv.VideoUrl).IsRequired().HasMaxLength(500);
                entity.Property(mv => mv.ThumbnailUrl).HasMaxLength(500);
                entity.Property(mv => mv.IsActive).HasDefaultValue(true); 
                entity.Property(mv => mv.DisplayOrder).HasDefaultValue(0);

                entity.HasOne(mv => mv.Movie)
                    .WithMany(m => m.MovieVideos)
                    .HasForeignKey(mv => mv.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(mv => mv.MovieId);
                entity.HasIndex(mv => mv.VideoType);

                entity.HasQueryFilter(mvt => !mvt.IsDeleted);
            });

            modelBuilder.Entity<MovieVideoTranslation>(entity =>
            {
                entity.HasKey(mvt => mvt.Id);

                entity.Property(mvt => mvt.Title).IsRequired().HasMaxLength(200);
                entity.Property(mvt => mvt.Description).HasMaxLength(1000);

                entity.HasOne(mvt => mvt.MovieVideo)
                    .WithMany(mv => mv.MovieVideoTranslations)
                    .HasForeignKey(mvt => mvt.MovieVideoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mvt => mvt.Language)
                    .WithMany()
                    .HasForeignKey(mvt => mvt.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(mvt => new { mvt.MovieVideoId, mvt.LanguageId }).IsUnique();

                entity.HasQueryFilter(mvt => !mvt.IsDeleted);
            });

            modelBuilder.Entity<MovieSocialLink>(entity =>
            {
                entity.HasKey(sl => sl.Id);

                entity.Property(sl => sl.Platform).IsRequired().HasMaxLength(50);
                entity.Property(sl => sl.Url).IsRequired().HasMaxLength(500);
                entity.Property(sl => sl.IconClass).HasMaxLength(100);
                entity.Property(sl => sl.IsActive).HasDefaultValue(true); 
                entity.Property(sl => sl.DisplayOrder).HasDefaultValue(0); 

                entity.HasOne(sl => sl.Movie)
                    .WithMany(m => m.MovieSocialLinks)
                    .HasForeignKey(sl => sl.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
          
                entity.HasIndex(sl => sl.MovieId);
                entity.HasIndex(sl => sl.Platform); 

                entity.HasQueryFilter(sl => !sl.IsDeleted);
            });

            #endregion

            #region Event Configuration

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Artists)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "EventArtists",
                    j => j.HasOne<Person>().WithMany().HasForeignKey("PersonId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Event>().WithMany().HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventTranslations)
                .WithOne(et => et.Event)
                .HasForeignKey(et => et.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventTranslation>()
                .HasOne(et => et.Language)
                .WithMany()
                .HasForeignKey(et => et.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventTranslation>()
                .HasIndex(et => new { et.EventId, et.LanguageId })
                .IsUnique();

            #endregion

            #region Sport Configuration

            modelBuilder.Entity<Sport>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(s => s.CoverImageUrl)
                    .HasMaxLength(500);

                entity.Property(s => s.Venue)
                    .HasMaxLength(200);

                entity.Property(s => s.GoogleMapsUrl)
                    .HasMaxLength(500);

                entity.Property(s => s.ContactPhone)
                    .HasMaxLength(20);

                entity.Property(s => s.ContactEmail)
                    .HasMaxLength(100);

                entity.Property(s => s.Categories)
                    .HasMaxLength(500); 

                entity.Property(s => s.Languages)
                    .HasMaxLength(200); 

                entity.Property(s => s.AgeRestriction)
                    .HasMaxLength(10); 
                entity.Property(s => s.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);

                entity.Property(s => s.IsFeatured)
                    .HasDefaultValue(false);

                entity.HasIndex(s => s.EventDate);
                entity.HasIndex(s => s.IsActive);
                entity.HasIndex(s => s.IsFeatured);
                entity.HasIndex(s => s.IsDeleted);

                entity.HasQueryFilter(s => !s.IsDeleted);
            });

            modelBuilder.Entity<Sport>()
                .HasMany(s => s.Players)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "SportPlayers",
                    j => j.HasOne<Person>()
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade), 
                    j => j.HasOne<Sport>()
                        .WithMany()
                        .HasForeignKey("SportId")
                        .OnDelete(DeleteBehavior.Cascade), 
                    j =>
                    {
                        j.HasKey("SportId", "PersonId");
                        j.HasIndex("PersonId");
                        j.HasIndex("SportId");
                    }
                );

            modelBuilder.Entity<Sport>()
                .HasOne(s => s.Currency)
                .WithMany()
                .HasForeignKey(s => s.CurrencyId)
                .OnDelete(DeleteBehavior.SetNull); 

            modelBuilder.Entity<Sport>()
                .HasMany(s => s.SportTranslations)
                .WithOne(st => st.Sport)
                .HasForeignKey(st => st.SportId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<SportTranslation>(entity =>
            {
                entity.HasKey(st => st.Id);

                entity.Property(st => st.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(st => st.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(st => st.Location)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasIndex(st => new { st.SportId, st.LanguageId })
                    .IsUnique();

                entity.HasIndex(st => st.LanguageId);


                entity.HasQueryFilter(st => !st.IsDeleted);
            });

            modelBuilder.Entity<SportTranslation>()
                .HasOne(st => st.Language)
                .WithMany()
                .HasForeignKey(st => st.LanguageId)
                .OnDelete(DeleteBehavior.Restrict); 

            #endregion

            #region Review Configuration

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

            #endregion

            #region ReviewReaction Configuration

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

            #endregion
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
