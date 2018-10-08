﻿using DataAccess.Interfaces.Repositories;
using MCT.DataAccess.Models;
using MCT.DataAccess.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace RESTAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Mitigating Circumstance REST API",
                    Description = "RESTful webservice for the Mitigating Circumstances Tracking system.",
                    Version = "v1"
                });
            });

            // SQL Server database connection
            services.AddDbContext<EFContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLServer"));
            });

            // Add repository implementations readys for dependency injection
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}