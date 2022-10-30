﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Eu.EDelivery.AS4.Fe.Logging
{
    public class ApplicationInsightsSetup : IApplicationInsightsSetup
    {
        private readonly IWebHostEnvironment env;

        public ApplicationInsightsSetup(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public void Run(IConfigurationBuilder configBuilder, IServiceCollection services, IConfigurationRoot localConfig)
        {
            if (env.IsEnvironment("Development")) configBuilder.AddApplicationInsightsSettings();
        }

        public void Run(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);
        }
    }
}