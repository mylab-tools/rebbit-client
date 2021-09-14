﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyLab.RabbitClient;
using MyLab.RabbitClient.Connection;
using MyLab.RabbitClient.Publishing;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods for integration into application
    /// </summary>
    public static partial class AppIntegration
    {
        /// <summary>
        /// Adds Rabbit services
        /// </summary>
        public static IServiceCollection AddRabbit(this IServiceCollection srv, RabbitConnectionStrategy connectionStrategy = RabbitConnectionStrategy.Lazy)
        {
            srv.AddSingleton<IRabbitChannelProvider, RabbitChannelProvider>()
                .AddSingleton<IRabbitPublisher, RabbitPublisher>();
            
            switch (connectionStrategy)
            {
                case RabbitConnectionStrategy.Lazy:
                    srv.AddSingleton<IRabbitConnectionProvider, LazyRabbitConnectionProvider>();
                    break;
                case RabbitConnectionStrategy.Background:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionStrategy), "Rabbit connection strategy must be defined");
            }

            return srv;
        }

        /// <summary>
        /// Configures Rabbit Client
        /// </summary>
        public static IServiceCollection ConfigureRabbit(this IServiceCollection srv, IConfiguration cfg, string sectionName = "MQ")
        {
            return srv.Configure<RabbitOptions>(cfg.GetSection(sectionName));
        }

        /// <summary>
        /// Configures Rabbit Client
        /// </summary>
        public static IServiceCollection ConfigureRabbit(this IServiceCollection srv, Action<RabbitOptions> configureAct)
        {
            return srv.Configure(configureAct);
        }
    }
}
