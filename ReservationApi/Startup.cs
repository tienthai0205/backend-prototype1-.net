using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ReservationApi.Models;

namespace ReservationApi
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddDbContext<UserContext>( opt => opt.UseInMemoryDatabase("Users"));
            services.AddAuthentication("OAuth")
                    .AddJwtBearer("OAuth", config =>
                    {
                        var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
                        var key = new SymmetricSecurityKey(secretBytes);

                        // config.Events = new JwtBearerEvents()
                        // {
                        //     OnMessageReceived = context => {
                        //         if(context.Request.Query.ContainsKey("access_token"))
                        //         {
                        //             context.Token = context.Request.Query["access_token"];
                        //         }
                        //         return Task.CompletedTask;
                        //     }
                        // };
                        config.TokenValidationParameters = new TokenValidationParameters()
                        {
                            IssuerSigningKey = key,
                            ValidIssuer = Constants.Issuer,
                            ValidAudience = Constants.Audiance 
                        };
                    });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
