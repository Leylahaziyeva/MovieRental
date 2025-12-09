using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.Repositories;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.DAL
{
    public static class DataAccessLayerServiceRegistration
    {
        public static IServiceCollection AddDataAccessLayerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }));

            services.AddScoped(typeof(IRepositoryAsync<>), typeof(EfCoreRepository<>));

            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ISliderRepository, SliderRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
            services.AddScoped<ISportRepository, SportRepository>();
            services.AddScoped<ISportTypeRepository, SportTypeRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();

            services.AddScoped<DataInitializer>();

            return services;
        }
    }
}
