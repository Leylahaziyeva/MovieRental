using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using MovieRental.BLL.Mapping;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.Services.Implementations;
using System.Globalization;

namespace MovieRental.BLL
{
    public static class BusinessLogicLayerServiceRegistration
    {
        public static IServiceCollection AddBusinessLogicLayerServices(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(
               options =>
               {
                   var supportedCultures = new List<CultureInfo>
                       {
                        new CultureInfo("en-US"),
                        new CultureInfo("az")
                       };

                   options.DefaultRequestCulture = new RequestCulture(culture: "az", uiCulture: "az");

                   options.SupportedCultures = supportedCultures;
                   options.SupportedUICultures = supportedCultures;

               });

            services.AddAutoMapper(confg => confg.AddProfile<MappingProfile>());

            services.AddScoped(typeof(ICrudService<,,,>), typeof(CrudManager<,,,>));

            services.AddSingleton<StringLocalizerManager>();

            services.AddScoped<ICloudinaryService, CloudinaryManager>();
            services.AddScoped<ICookieService, CookieManager>();
            services.AddScoped<ILanguageService, LanguageManager>();
            services.AddScoped<ICurrencyService, CurrencyManager>();

            services.AddScoped<IAccountService, AccountManager>();
            services.AddScoped<IHomeService, HomeManager>();
            services.AddScoped<IHeaderService, HeaderManager>();
            services.AddScoped<ISearchHistoryService, SearchHistoryManager>();
            services.AddScoped<ISliderService, SliderManager>();
            services.AddScoped<IPersonService, PersonManager>();
            services.AddScoped<IPersonTranslationService, PersonTranslationManager>();
            services.AddScoped<IOfferService, OfferManager>();
            services.AddScoped<IGenreService, GenreManager>();
            services.AddScoped<IMovieService, MovieManager>();
            services.AddScoped<IWatchlistService, WatchlistManager>();
            services.AddScoped<IUserListService, UserListManager>();
            services.AddScoped<IRentalService, RentalManager>();
            services.AddScoped<IEventService, EventManager>();
            services.AddScoped<IEventCategoryService, EventCategoryManager>();
            services.AddScoped<ISportService, SportManager>();
            services.AddScoped<ISportTypeService, SportTypeManager>();
            services.AddScoped<ILocationService, LocationManager>();
            services.AddScoped<IFooterService, FooterManager>();

            //services.AddScoped<IProductTranslationService, ProductTranslationManager>();
            //services.AddScoped<IProductImageService, ProductImageManager>();
            //services.AddScoped<IDashboardService, DashboardManager>();
            //services.AddScoped<FileService>();
            //services.AddScoped<BasketManager>();

            return services;
        }
    }
}
