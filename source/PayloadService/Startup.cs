﻿using System.IO;
using System.Reflection;
using Eu.EDelivery.AS4.PayloadService.Infrastructure.SwaggerUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Eu.EDelivery.AS4.PayloadService
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
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder =
                new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                          .AddJsonFile("appsettings.json", false, true)
                                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSwagger();

            app.UseSwaggerUi(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"AS4.NET Payload Service Web API V{AssemblyVersion}"); });

            app.UseMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Title = "AS4.NET Payload Service",
                            Version = $"v{AssemblyVersion}",
                            Description = "A Web API to upload and download payloads in a persistent manner.",
                            TermsOfService = "None",
                            Contact = new Contact {Name = "DG EMPL" },
                            License =
                                new License
                                {
                                    Name = "EUPL License v1.1.",
                                    Url = "https://joinup.ec.europa.eu/community/eupl/og_page/european-union-public-licence-eupl-v11"
                                }
                        });

                    options.OperationFilter<FileUploadOperation>();
                    options.IncludeXmlComments(GetXmlCommentsPath());
                });
        }

        private string GetXmlCommentsPath()
        {
            ApplicationEnvironment app = PlatformServices.Default.Application;
            return Path.Combine(app.ApplicationBasePath, "payload-service-docs.xml");
        }
    }
}