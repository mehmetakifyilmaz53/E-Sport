using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Norsera.business.Abstract;
using Norsera.business.Concrete;
using Norsera.data.Abstract;
using Norsera.data.Concrete.SQL.EFCore;
using Norsera.Emailservices;
using Norsera.Identity;

namespace Norsera
{
    public class StartUp
    {
        private IConfiguration _configuration;
        public StartUp(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data Source=NorseraDb"));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //pasword
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;

                //Locout
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                //user
                // options.User.AllowedUserNameCharacters= "";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            //cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(5); //5 gün boyunda login olabilirsin
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".Norsera.Security.Cookie",
                    SameSite = SameSiteMode.Strict

                };

            });

            services.AddScoped<IProductRepository, EFCoreProductRepository>();
            services.AddScoped<ICategoryRepository, EFCoreCategoryRepository>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<ICartRepository, EfCoreCartRepository>();
            services.AddScoped<ICartService, CartManager>();

            services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"]
                        )
                    );

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseRouting(); //site içerisinde yönlendirme kullanılıcağını belirten kod
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                {

                    endpoints.MapControllerRoute(
                       name: "checkout",
                       pattern: "checkout",
                       defaults: new { controller = "Cart", Action = "Checkout" }
                   );

                    endpoints.MapControllerRoute(
                       name: "adminusers",
                       pattern: "user/menage/{UserName?}",
                       defaults: new { controller = "Admin", Action = "UserMenage" }
                   );


                    endpoints.MapControllerRoute(
                       name: "cart",
                       pattern: "cart",
                       defaults: new { controller = "Cart", Action = "Index" }
                   );


                    endpoints.MapControllerRoute(
                       name: "adminusers",
                       pattern: "admin/user/list",
                       defaults: new { controller = "Admin", Action = "UserList" }
                   );

                   endpoints.MapControllerRoute(
                       name: "adminusercreate",
                       pattern: "admin/user/create",
                       defaults: new { controller = "Admin", Action = "UserCreate" }
                   );

                    endpoints.MapControllerRoute(
                        name: "adminuseredit",
                        pattern: "admin/user/{id?}",
                        defaults: new { controller = "Admin", Action = "UserEdit" }
                    );


                    endpoints.MapControllerRoute(
                        name: "adminroles",
                        pattern: "admin/role/list",
                        defaults: new { controller = "Admin", Action = "RoleList" }
                    );

                    endpoints.MapControllerRoute(
                       name: "adminrolecreate",
                       pattern: "admin/role/create",
                       defaults: new { controller = "Admin", Action = "RoleCreate" }
                   );

                    endpoints.MapControllerRoute(
                       name: "adminroleedit",
                       pattern: "admin/role/{id?}",
                       defaults: new { controller = "Admin", Action = "RoleEdit" }
                   );

                    endpoints.MapControllerRoute(
                        name: "adminproducts",
                        pattern: "admin/products",
                        defaults: new { controller = "Admin", Action = "ProductList" }
                    );

                    endpoints.MapControllerRoute(
                        name: "adminproductcreate",
                        pattern: "admin/products/create",
                        defaults: new { controller = "Admin", Action = "ProductCreate" }
                    );

                    endpoints.MapControllerRoute(
                        name: "adminproductedit", //productedit
                        pattern: "admin/products/{id?}",
                        defaults: new { controller = "Admin", Action = "ProductEdit" } //ProductEdit
                    );


                    endpoints.MapControllerRoute(
                        name: "admincategories",
                        pattern: "admin/categories",
                        defaults: new { controller = "Admin", Action = "CategoryList" }
                    );

                    endpoints.MapControllerRoute(
                       name: "admincategorycreate",
                       pattern: "admin/categories/create",
                       defaults: new { controller = "Admin", Action = "CategoryCreate" }
                   );

                    endpoints.MapControllerRoute(
                        name: "admincategoryedit",
                        pattern: "admin/categories/{id?}",
                        defaults: new { controller = "Admin", Action = "CategoryEdit" }
                    );


                    endpoints.MapControllerRoute(
                        name: "search",
                        pattern: "search",
                        defaults: new { controller = "Shop", Action = "search" }
                    );

                    endpoints.MapControllerRoute(
                        name: "productdetails",
                        pattern: "{url}",
                        defaults: new { controller = "Shop", Action = "details" }
                    );

                    endpoints.MapControllerRoute(
                        name: "products",
                        pattern: "products/{category?}",
                        defaults: new { controller = "Shop", Action = "list" }
                    );

                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=shop}/{action=index}/{id?}" //= ile varsayılan bir değer vermeye default routing denir
                    );
                });
        }
    }
}