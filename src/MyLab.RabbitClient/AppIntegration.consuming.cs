﻿using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MyLab.RabbitClient.Consuming;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class AppIntegration
    {
        static readonly ServiceDescriptor ConsumerHostServiceDescriptor = new ServiceDescriptor(typeof(IHostedService), typeof(ConsumerHost), ServiceLifetime.Singleton);
        static readonly ServiceDescriptor ConsumerManagerServiceDescriptor = new ServiceDescriptor(typeof(IConsumerManager), typeof(ConsumerManager), ServiceLifetime.Singleton);

        /// <summary>
        /// Registers consumer for specified queue
        /// </summary>
        public static IServiceCollection AddRabbitConsumer(this IServiceCollection srvColl, string queue, IRabbitConsumer consumer)
        {
            return srvColl
                .TryAddConsuming()
                .AddRabbitConsumers(new SingleConsumerRegistrar(queue, consumer));
        }

        /// <summary>
        /// Registers consumer type for specified queue 
        /// </summary>
        public static IServiceCollection AddRabbitConsumer<TConsumer>(this IServiceCollection srvColl, string queue)
            where TConsumer : class, IRabbitConsumer
        {
            return srvColl
                .TryAddConsuming()
                .AddRabbitConsumers(new TypedConsumerRegistrar<TConsumer>(queue));
        }

        /// <summary>
        /// Registers consumer for queue which retrieve from options
        /// </summary>
        public static IServiceCollection AddRabbitConsumer<TOptions,TConsumer>(this IServiceCollection srvColl, Func<TOptions, string> queueProvider, bool optional = false)
            where TOptions : class, new()
            where TConsumer : class, IRabbitConsumer
        {
            return srvColl
                .TryAddConsuming()
                .AddRabbitConsumers(new OptionsConsumerRegistrar<TOptions, TConsumer>(queueProvider, optional));
        }

        /// <summary>
        /// Adds consumer registrar which allow to registers several consumers depends on dependent options and services
        /// </summary>
        public static IServiceCollection AddRabbitConsumers<TRegistrar>(this IServiceCollection srvColl)
            where TRegistrar : class, IRabbitConsumerRegistrar
        {
            return srvColl
                .TryAddConsuming()
                .AddRabbitConsumers(new WrapperConsumerRegistrar<TRegistrar>());
        }

        /// <summary>
        /// Adds consumer registrar which allow to registers several consumers depends on dependent options and services
        /// </summary>
        public static IServiceCollection AddRabbitConsumers(this IServiceCollection srvColl, IRabbitConsumerRegistrar registrar)
        {
            return srvColl
                .TryAddConsuming()
                .Configure<ConsumerRegistrarSource>(s => s.Add(registrar));
        }

        /// <summary>
        /// Adds consumed message context
        /// </summary>
        public static IServiceCollection AddRabbitCtx<T>(this IServiceCollection srvColl)
            where T : class, IConsumingContext
        {
            return srvColl.AddSingleton<IConsumingContext, T>();
        }

        /// <summary>
        /// Adds consumed message context
        /// </summary>
        public static IServiceCollection AddRabbitCtx(this IServiceCollection srvColl, IConsumingContext context)
        {
            return srvColl.AddSingleton<IConsumingContext>(context);
        }

        private static IServiceCollection TryAddConsuming(this IServiceCollection srvColl)
        {
            srvColl.TryAdd(ConsumerHostServiceDescriptor);
            srvColl.TryAdd(ConsumerManagerServiceDescriptor);

            return srvColl;
        }
    }
}