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
        public required DbSet<LanguageTranslation> LanguageTranslations { get; set; }
        public required DbSet<Currency> Currencies { get; set; }
        public required DbSet<CurrencyTranslation> CurrencyTranslations { get; set; }
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
        public required DbSet<UserList> UserLists { get; set; }
        public required DbSet<UserListMovie> UserListMovies { get; set; }
        public required DbSet<UserWatchlist> UserWatchlists { get; set; }
        public required DbSet<WatchHistory> WatchHistories { get; set; }
        public required DbSet<Genre> Genres { get; set; }
        public required DbSet<GenreTranslation> GenreTranslations { get; set; }
        public required DbSet<Event> Events { get; set; }
        public required DbSet<EventTranslation> EventTranslations { get; set; }
        public required DbSet<EventCategory> EventCategories { get; set; }
        public required DbSet<EventCategoryTranslation> EventCategoryTranslations { get; set; }
        public required DbSet<Sport> Sports { get; set; }
        public required DbSet<SportTranslation> SportTranslations { get; set; }
        public required DbSet<SportType> SportTypes { get; set; }
        public required DbSet<SportTypeTranslation> SportTypeTranslations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationTranslation> LocationTranslations { get; set; }
        public required DbSet<Person> Persons { get; set; }
        public required DbSet<PersonTranslation> PersonTranslations { get; set; }
        public required DbSet<Review> Reviews { get; set; }
        public required DbSet<ReviewReaction> ReviewReactions { get; set; }
        public required DbSet<ShareLog> ShareLogs { get; set; }
        public required DbSet<Rental> Rentals { get; set; }
        public required DbSet<Offer> Offers { get; set; }
        public required DbSet<OfferTranslation> OfferTranslations { get; set; }
        public required DbSet<Notification> Notifications { get; set; }
        public required DbSet<NotificationTranslation> NotificationTranslations { get; set; }
        public required DbSet<SearchHistory> SearchHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Currency Translation Configuration

            modelBuilder.Entity<CurrencyTranslation>()
                .HasOne(ct => ct.Currency)
                .WithMany(c => c.Translations)
                .HasForeignKey(ct => ct.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<CurrencyTranslation>()
                .HasOne(ct => ct.Language)
                .WithMany()  
                .HasForeignKey(ct => ct.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<CurrencyTranslation>()
                .HasIndex(ct => new { ct.CurrencyId, ct.LanguageId })
                .IsUnique();

            #endregion

            #region Language Translation Configuration

            modelBuilder.Entity<LanguageTranslation>()
                .HasOne(lt => lt.Language)
                .WithMany(l => l.LanguageTranslations)
                .HasForeignKey(lt => lt.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LanguageTranslation>()
                .HasOne(lt => lt.TranslationLanguage)
                .WithMany()  
                .HasForeignKey(lt => lt.TranslationLanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LanguageTranslation>()
                .HasIndex(lt => new { lt.LanguageId, lt.TranslationLanguageId })
                .IsUnique();

            #endregion

            #region UserWatchlist Configuration

            modelBuilder.Entity<UserWatchlist>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Movie)
                    .WithMany()
                    .HasForeignKey(e => e.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.MovieId }).IsUnique();
            });

            #endregion

            #region UserList Configuration

            modelBuilder.Entity<UserList>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
            });

            #endregion

            #region UserListMovie Configuration

            modelBuilder.Entity<UserListMovie>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.UserList)
                    .WithMany(ul => ul.UserListMovies)
                    .HasForeignKey(e => e.UserListId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Movie)
                    .WithMany()
                    .HasForeignKey(e => e.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserListId, e.MovieId }).IsUnique();
            });

            #endregion

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

            #region Rental Configuration

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(r => r.Status)
                    .HasConversion<string>();

                entity.Property(r => r.PaymentIntentId)
                    .HasMaxLength(255);

                entity.Property(r => r.TransactionId)
                    .HasMaxLength(255);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Rentals)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Movie)
                    .WithMany(m => m.Rentals)
                    .HasForeignKey(r => r.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => r.MovieId);
                entity.HasIndex(r => r.Status);
                entity.HasIndex(r => r.ExpiryDate);
                entity.HasIndex(r => new { r.UserId, r.Status });

                entity.HasQueryFilter(r => !r.IsDeleted);
            });

            #endregion

            #region WatchHistory Configuration

            modelBuilder.Entity<WatchHistory>(entity =>
            {
                entity.HasKey(wh => wh.Id);

                entity.Property(wh => wh.CompletionPercentage)
                    .HasColumnType("decimal(5,2)");

                entity.Property(wh => wh.DeviceType)
                    .HasMaxLength(50);

                entity.Property(wh => wh.IpAddress)
                    .HasMaxLength(45);

                entity.HasOne(wh => wh.User)
                    .WithMany(u => u.WatchHistories)
                    .HasForeignKey(wh => wh.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wh => wh.Movie)
                    .WithMany()
                    .HasForeignKey(wh => wh.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wh => wh.Rental)
                    .WithMany(r => r.WatchHistories)
                    .HasForeignKey(wh => wh.RentalId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(wh => wh.UserId);
                entity.HasIndex(wh => wh.MovieId);
                entity.HasIndex(wh => wh.RentalId);
                entity.HasIndex(wh => wh.WatchedAt);
                entity.HasIndex(wh => new { wh.UserId, wh.WatchedAt });

                entity.HasQueryFilter(wh => !wh.IsDeleted);
            });

            #endregion

            #region Event Configuration 

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CoverImageUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(50);

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(200);

                entity.Property(e => e.Venue)
                    .HasMaxLength(500);

                entity.Property(e => e.GoogleMapsUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.AgeRestriction)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.IsFeatured)
                    .HasDefaultValue(false);

                entity.HasOne(e => e.Currency)
                    .WithMany()
                    .HasForeignKey(e => e.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EventCategory)
                    .WithMany(ec => ec.Events)
                    .HasForeignKey(e => e.EventCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Events)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Artists)
                    .WithMany()
                    .UsingEntity<Dictionary<string, object>>(
                        "EventArtists", 
                        j => j
                            .HasOne<Person>()
                            .WithMany()
                            .HasForeignKey("ArtistsId") 
                            .OnDelete(DeleteBehavior.Cascade),
                        j => j
                            .HasOne<Event>()
                            .WithMany()
                            .HasForeignKey("EventsId") 
                            .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("EventsId", "ArtistsId"); 
                            j.ToTable("EventArtists");
                            j.HasIndex("ArtistsId"); 
                        });

                entity.HasIndex(e => e.EventDate);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsFeatured);
                entity.HasIndex(e => e.EventCategoryId);
                entity.HasIndex(e => e.LocationId);
                entity.HasIndex(e => e.IsDeleted);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<EventTranslation>(entity =>
            {
                entity.HasKey(et => et.Id);

                entity.Property(et => et.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(et => et.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(et => et.Event)
                    .WithMany(e => e.EventTranslations)
                    .HasForeignKey(et => et.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(et => et.Language)
                    .WithMany()
                    .HasForeignKey(et => et.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(et => new { et.EventId, et.LanguageId })
                    .IsUnique();

                entity.HasQueryFilter(et => !et.IsDeleted);
            });

            #endregion

            //#region Event Configuration

            //modelBuilder.Entity<Event>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.Property(e => e.ImageUrl)
            //        .IsRequired()
            //        .HasMaxLength(500);

            //    entity.Property(e => e.CoverImageUrl)
            //        .HasMaxLength(500);

            //    entity.Property(e => e.Price)
            //        .HasColumnType("decimal(18,2)");

            //    entity.Property(e => e.ContactPhone)
            //        .HasMaxLength(50);

            //    entity.Property(e => e.ContactEmail)
            //        .HasMaxLength(200);

            //    entity.Property(e => e.Venue)
            //        .HasMaxLength(500);

            //    entity.Property(e => e.GoogleMapsUrl)
            //        .HasMaxLength(500);

            //    entity.Property(e => e.AgeRestriction)
            //        .HasMaxLength(50);

            //    entity.Property(e => e.IsActive)
            //        .HasDefaultValue(true);

            //    entity.Property(e => e.IsFeatured)
            //        .HasDefaultValue(false);

            //    entity.HasOne(e => e.Currency)
            //        .WithMany()
            //        .HasForeignKey(e => e.CurrencyId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(e => e.EventCategory)
            //        .WithMany(ec => ec.Events)
            //        .HasForeignKey(e => e.EventCategoryId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(e => e.Location)
            //        .WithMany(l => l.Events)
            //        .HasForeignKey(e => e.LocationId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasMany(e => e.Artists)
            //        .WithMany()
            //        .UsingEntity(j => j.ToTable("EventArtists"));

            //    entity.HasIndex(e => e.EventDate);
            //    entity.HasIndex(e => e.IsActive);
            //    entity.HasIndex(e => e.IsFeatured);
            //    entity.HasIndex(e => e.EventCategoryId);
            //    entity.HasIndex(e => e.LocationId);
            //    entity.HasIndex(e => e.IsDeleted);

            //    entity.HasQueryFilter(e => !e.IsDeleted);
            //});

            //modelBuilder.Entity<EventTranslation>(entity =>
            //{
            //    entity.HasKey(et => et.Id);

            //    entity.Property(et => et.Name)
            //        .IsRequired()
            //        .HasMaxLength(200);

            //    entity.Property(et => et.Description)
            //        .IsRequired()
            //        .HasMaxLength(2000);

            //    entity.HasOne(et => et.Event)
            //        .WithMany(e => e.EventTranslations)
            //        .HasForeignKey(et => et.EventId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(et => et.Language)
            //        .WithMany()
            //        .HasForeignKey(et => et.LanguageId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasIndex(et => new { et.EventId, et.LanguageId })
            //        .IsUnique();

            //    entity.HasQueryFilter(et => !et.IsDeleted);
            //});

            //#endregion

            #region Sport Configuration

            modelBuilder.Entity<Sport>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(s => s.CoverImageUrl)
                    .HasMaxLength(500);

                entity.Property(s => s.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(s => s.ContactPhone)
                    .HasMaxLength(50);

                entity.Property(s => s.ContactEmail)
                    .HasMaxLength(200);

                entity.Property(s => s.Venue)
                    .HasMaxLength(500);

                entity.Property(s => s.GoogleMapsUrl)
                    .HasMaxLength(500);

                entity.Property(s => s.AgeRestriction)
                    .HasMaxLength(50);

                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);

                entity.Property(s => s.IsFeatured)
                    .HasDefaultValue(false);

                entity.HasOne(s => s.Currency)
                    .WithMany()
                    .HasForeignKey(s => s.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.SportType)
                    .WithMany(st => st.Sports)
                    .HasForeignKey(s => s.SportTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Location)
                    .WithMany(l => l.Sports)
                    .HasForeignKey(s => s.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.EventDate);
                entity.HasIndex(s => s.IsActive);
                entity.HasIndex(s => s.IsFeatured);
                entity.HasIndex(s => s.SportTypeId);
                entity.HasIndex(s => s.LocationId);
                entity.HasIndex(s => s.IsDeleted);

                entity.HasQueryFilter(s => !s.IsDeleted);
            });

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
                    .HasMaxLength(500);

                entity.HasOne(st => st.Sport)
                    .WithMany(s => s.SportTranslations)
                    .HasForeignKey(st => st.SportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(st => st.Language)
                    .WithMany()
                    .HasForeignKey(st => st.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(st => new { st.SportId, st.LanguageId })
                    .IsUnique();

                entity.HasQueryFilter(st => !st.IsDeleted);
            });

            #endregion

            #region Location Configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.CreatedAt)
                    .IsRequired();

                entity.Property(l => l.UpdatedAt)
                    .IsRequired();

                entity.HasIndex(l => l.IsDeleted);
                entity.HasQueryFilter(l => !l.IsDeleted);
            });

            modelBuilder.Entity<LocationTranslation>(entity =>
            {
                entity.HasKey(lt => lt.Id);

                entity.Property(lt => lt.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(lt => new { lt.LocationId, lt.LanguageId })
                    .IsUnique();

                entity.HasQueryFilter(lt => !lt.IsDeleted);
            });

            modelBuilder.Entity<LocationTranslation>()
                .HasOne(lt => lt.Location)
                .WithMany(l => l.Translations)
                .HasForeignKey(lt => lt.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LocationTranslation>()
                .HasOne(lt => lt.Language)
                .WithMany()
                .HasForeignKey(lt => lt.LanguageId)
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
