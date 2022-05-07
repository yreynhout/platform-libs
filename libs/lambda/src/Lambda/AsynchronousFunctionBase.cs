﻿using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda;

/// <summary>
/// A base asynchronous function (using the Event invocation type), that encapsulates
/// some boiler plate code around configuration, logging and setting up dependency injection 
/// 
/// By default configuration is loaded from environment variables, appsettings.json and appsettings.{environment}.json
/// </summary>
/// <typeparam name="THandler">The handler that will be activated to handle the request.</typeparam>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TOptions">The options type.</typeparam>
public abstract class AsynchronousFunctionBase<TRequest, TOptions, THandler> : FunctionBase<TOptions, THandler>
    where THandler : class, IAsynchronousHandler<TRequest> where TOptions : class, new()
{
    /// <summary>
    /// Initializes a new instance of <see cref="SynchronousFunctionBase{TConfig, TRequest, TResponse, THandler}"/>
    /// </summary>
    /// <param name="configureConfiguration">
    ///     An action to configure the configuration. By default appsettings.json,
    ///     appsettings.{environment}.json and environment variables providers are added.
    /// </param>
    /// <param name="configurelogging">
    ///     An acction to configure logging services. By default the Lambda Logger provider
    ///     is added and the minimum logging level isset to 'Information'.
    /// </param>
    /// <param name="configureServices">
    ///     Configure any services needed for injection into your handler. By default,
    ///     an instance of TConfig is added as a singleton and THandler is added
    ///     as transient.
    /// </param>
    /// <param name="environmentVariablesPrefix">
    ///     A prefix to use for environment variables. Defaults to empty string.
    /// </param>
    protected AsynchronousFunctionBase(
        Action<IConfigurationBuilder>? configureConfiguration     = null,
        Action<ILoggingBuilder>?       configurelogging           = null,
        Action<IServiceCollection>?    configureServices          = null,
        string                         environmentVariablesPrefix = "")
        : base(configureConfiguration, configurelogging, configureServices, environmentVariablesPrefix)
    { }

    public Task Handle(TRequest input, ILambdaContext context)
    {
        var handler = ServiceProvider.GetRequiredService<THandler>();
        return handler.Handle(input, context);
    }
}