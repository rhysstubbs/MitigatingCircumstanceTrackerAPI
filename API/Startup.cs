using MCT.RESTAPI.Models.GoogleCloud;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NotificationProvider;
using NotificationProvider.Interfaces;
using NotificationProvider.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace RESTAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appUri = new UriBuilder()
            {
                Scheme = "https",
                Host = Configuration["CORS:Origin"].ToString(),
            };

            // CORS Policy Configuration
            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentPolicy",
                    builder => builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials()
                                        .Build());

                options.AddPolicy("ProductionPolicy",
                    builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                                        .WithOrigins("https://*.rhysstubbs.services")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials()
                                        .Build());
            });

            // Swagger Configuration
            services.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
                c.DescribeStringEnumsInCamelCase();

                c.SwaggerDoc("v1", new Info
                {
                    Title = "Mitigating Circumstance REST API",
                    Description = "RESTful webservice for the Mitigating Circumstances Tracking system.",
                    Version = "v1"
                });
            });

            // Services Registration for DI
            services.AddOptions();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<INotificationService, NotificationService>();
            services.Configure<CloudStorageOptions>(Configuration.GetSection("GoogleCloudStorage"));
            services.AddScoped<Slack>();

            // Finally, Add and Configure the MVC Framework
            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("DevelopmentPolicy");

                app.UseDeveloperExceptionPage();

                app.UseDatabaseErrorPage();

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(Directory.GetCurrentDirectory(), Configuration["GAE:Credentials"]));
            }
            else
            {
                app.UseExceptionHandler("/Error");

                app.UseCors("ProductionPolicy");

                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = string.Empty;
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}