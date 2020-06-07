using System.IO;
using System.Reflection;
using System.Text.Json;
using FluentValidation;
using Hahn.ApplicationProcess.May2020.Data;
using Hahn.ApplicationProcess.May2020.Data.Repositories;
using Hahn.ApplicationProcess.May2020.Domain.Models;
using Hahn.ApplicationProcess.May2020.Domain.Services;
using Hahn.ApplicationProcess.May2020.Domain.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace Hahn.ApplicationProcess.May2020.Web {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            /* The db context for EF access */
            services.AddDbContext<GenericDbContext<Applicant>>(options => options.UseInMemoryDatabase(databaseName: "ApplicantsDb"));

            /* Validation may be used as singleton, this also lets the cache survive requests */
            services.AddSingleton<AbstractValidator<Applicant>, ApplicantValidator>();

            /* Applicant repository + service as scoped */
            services.AddScoped<IGenericRepository<Applicant>, GenericRepository<Applicant>>();
            services.AddScoped<IGenericService<Applicant>, GenericService<Applicant>>();

            /* Build swagger api documentation */
            services.AddMvcCore()
                .AddApiExplorer();
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1.0.0",
                    Title = "application-process-api-v1",
                    Description = "Api to interact with the hahn applicants service",
                });
                options.EnableAnnotations();
                options.ExampleFilters();
            });
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

            services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                /* Host swagger api documentation */
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "application-process-api-v1");
                });
            }

            /* Enable strict https */
            app.UseHttpsRedirection();
            app.UseHsts();

            /* Generic middleware */
            app.UseRouting();
            app.UseAuthorization();

            /* Serve frontend as static */
            var frontendPath = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") /* published, does not have to be production build ! */
                : Path.Combine(Directory.GetCurrentDirectory(), "Frontend", "dist"); /* running from source */
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(frontendPath),
            });

            /* Include serilog request logging */
            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints => {
                /* Send the user to frontend on root-request  */
                endpoints.MapGet("/", async context => {
                    context.Response.Redirect("index.html", true);
                });
                /* Use controllers */
                endpoints.MapControllers();
            });
        }
    }
}
