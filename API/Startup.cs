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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            options.AddPolicy("MyPolicy",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
                c.DescribeStringEnumsInCamelCase();

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.SwaggerDoc("v1", new Info
                {
                    Title = "Mitigating Circumstance REST API",
                    Description = "RESTful webservice for the Mitigating Circumstances Tracking system.",
                    Version = "v1"
                });
            });

            services.AddOptions();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<INotificationService, NotificationService>();
            services.Configure<CloudStorageOptions>(Configuration.GetSection("GoogleCloudStorage"));
            services.AddScoped<Slack>();

            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(Directory.GetCurrentDirectory(), Configuration["GAE:Credentials"]));
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = string.Empty;
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}