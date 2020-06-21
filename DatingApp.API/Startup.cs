using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseSqlite
            (Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            services.AddCors();
            // AddScoped means that one instance is created per HTTP request,
            // but uses the same instance in other calls within the same request....
            // Specifies Interface, and specific instance of the interface used.
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Using JSON Web Token Authentication.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Check if key is valid
                        ValidateIssuerSigningKey = true,
                        // Checks for value of key inside appsettings.json file, encoded from 
                        // string to bytearray.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        // In this case both issuer and audience are localhost, no need to validate
                        ValidateIssuer = false,
                        ValidateAudience = false

                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Use developer exception handler
            if (env.IsDevelopment())
            {
                // Returns developer exception page.
                app.UseDeveloperExceptionPage();
            }
            // Use global exception handler
            else
            {
                // Adds middleware to the pipeline which catches exceptions, logs 
                // them, and re-executes the request in an alternate pipeline.
                app.UseExceptionHandler(builder =>
                {
                    // Access the context (related to our http request and response)
                    builder.Run(async context =>{

                        // Here we control the status code we return when we 
                        // handle an exception.
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        // Get details of error and store in variable 'error'
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if(error != null)
                        { 
                            // Adds a new header into our response based on the Extensions
                            // C# class in the Helpers folder.
                            context.Response.AddApplicationError(error.Error.Message);
                            // Writing the error message into the http response as well.
                            // We want to modify 'Response' from the exception, by extending 
                            // this 'Response' to add custom application errors to the response.
                            // We ant to add a "AddApplicationError()" function to Response.
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            // Attempts to redirect any HTTP requests to HTTPS 
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}