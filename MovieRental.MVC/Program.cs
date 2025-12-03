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
            //builder.Services.AddHttpContextAccessor();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //FilePathConstants.ProductImagePath = Path.Combine(builder.Environment.WebRootPath, "images", "products");
            //FilePathConstants.ProfileImagePath = Path.Combine(builder.Environment.WebRootPath, "images", "users");

            var app = builder.Build();

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

            // Admin Role and User 
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                string adminRole = "Admin";
                string adminEmail = "admin@moviebok.com";  
                string adminPassword = "Admin@123456";      

                if (!await roleManager.RoleExistsAsync(adminRole))
                    await roleManager.CreateAsync(new IdentityRole(adminRole));

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin",      
                        LastName = "User",  
                        IsActive = true         
                    };
                    await userManager.CreateAsync(adminUser, adminPassword);
                }

                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                    await userManager.AddToRoleAsync(adminUser, adminRole);
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();  

            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();

            app.UseRequestLocalization(locOptions!.Value);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
