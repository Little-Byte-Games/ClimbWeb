using System.Reflection;
using Climb.Data;
using Climb.Extensions;
using Climb.Services;
using Climb.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema;
using NSwag.AspNetCore;

namespace Climb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "climb.com",
                        ValidateAudience = true,
                        ValidAudience = "climb",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = Configuration.GetSecurityKey(),
                        ValidateLifetime = true,
                    };
                    options.SaveToken = true;
                });

            services.AddMvc();

            services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();

            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddTransient<ITokenHelper, TokenHelper>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
                app.UseDatabaseErrorPage();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly,
                settings => settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase);
            app.UseMvc();
        }
    }
}