using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MovieRental.BLL;
using MovieRental.DAL;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;


namespace MovieRental.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddMvc().AddViewLocalization();


            builder.Services.AddDataAccessLayerServices(builder.Configuration);
            builder.Services.AddBusinessLogicLayerServices();


            //var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            //builder.Services.AddControllersWithViews()
            //    .AddViewLocalization()
            //    .AddDataAnnotationsLocalization();


            //builder.Services.AddDataAccessLayerServices(builder.Configuration);
            //builder.Services.AddBussinessLogicLayerServices();
            //builder.Services.AddHttpContextAccessor();

            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //FilePathConstants.ProductImagePath = Path.Combine(builder.Environment.WebRootPath, "images", "products");
            //FilePathConstants.ProfileImagePath = Path.Combine(builder.Environment.WebRootPath, "images", "users");
            //FilePathConstants.CategoryImagePath = Path.Combine(builder.Environment.WebRootPath, "images", "collections");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var dataInitializer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
                await dataInitializer.InitializeAsync();
            }

            //// Admin Role and User 
            //using (var scope = app.Services.CreateScope())
            //{
            //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            //    string adminRole = "Admin";
            //    string adminEmail = "admin@example.com";
            //    string adminPassword = "Admin@123";

            //    if (!await roleManager.RoleExistsAsync(adminRole))
            //        await roleManager.CreateAsync(new IdentityRole(adminRole));

            //    var adminUser = await userManager.FindByEmailAsync(adminEmail);
            //    if (adminUser == null)
            //    {
            //        adminUser = new AppUser
            //        {
            //            UserName = adminEmail,
            //            Email = adminEmail,
            //            EmailConfirmed = true
            //        };
            //        await userManager.CreateAsync(adminUser, adminPassword);
            //    }

            //    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            //        await userManager.AddToRoleAsync(adminUser, adminRole);
            //}


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // var supportedCultures = new[]
            //{
            //     new CultureInfo("az-AZ"),
            //     new CultureInfo("en-US")
            // };

            // var localizationOptions = new RequestLocalizationOptions
            // {
            //     DefaultRequestCulture = new RequestCulture("az-AZ"),
            //     SupportedCultures = supportedCultures,
            //     SupportedUICultures = supportedCultures
            // };

            // localizationOptions.RequestCultureProviders.Clear();
            // localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());

            // app.UseRequestLocalization(localizationOptions);

            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();

            app.UseRequestLocalization(locOptions!.Value);


            app.UseRouting();

            //app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.MapControllerRoute(
            //   name: "areas",
            //   pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
