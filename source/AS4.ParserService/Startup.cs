using System;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;


namespace AS4.ParserService
{
    /// <summary>
    /// The start point class for the Payload Service Web API.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        public Startup(IWebHostEnvironment env)
        {
            IConfigurationBuilder builder =
                new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                          .AddJsonFile("./bin/appsettings.parserservice.json", true)
                                          .AddJsonFile($"./appsettings.parserservice.json", true)
                                          .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private string AssemblyVersion => GetType().GetTypeInfo().Assembly.GetName().Version.ToString();

        /// <summary>
        /// Gets the <see cref="IConfigurationRoot" /> implementation for the Payload Service Web API.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.s
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger Factory.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime appLifetime)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"AS4.NET Parser Service Web API V{AssemblyVersion}");
            });

            app.UseMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            //services.AddApplicationInsightsTelemetry();

            // Add framework services.
            services.AddMvc(x => x.EnableEndpointRouting = false);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "AS4.NET Parser Service",
                            Version = $"v{AssemblyVersion}",
                            Description = "A Web API to encode and decode AS4 messages.",
                            TermsOfService = new Uri("https://joinup.ec.europa.eu/community/eupl/tos"),
                            Contact = new OpenApiContact { Name = "DG EMPL" },
                            License =
                                new OpenApiLicense
                                {
                                    Name = "EUPL License v1.1.",
                                    Url = new Uri("https://joinup.ec.europa.eu/community/eupl/og_page/european-union-public-licence-eupl-v11")
                                }
                        });

                    //options.OperationFilter<FileUploadOperation>();
                    options.IncludeXmlComments(GetXmlCommentsPath());
                }
                );
        }

        private static string GetXmlCommentsPath()
        {
            const string xml = "AS4.ParserService.xml";
            var app = PlatformServices.Default.Application;
            var binPath = Path.Combine(app.ApplicationBasePath, "bin", xml);
            return File.Exists(binPath) ? binPath : Path.Combine(app.ApplicationBasePath, xml);
        }
    }
}