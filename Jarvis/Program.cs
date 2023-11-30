using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Jarvis.Services;

class Program
{
    static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                configBuilder.AddUserSecrets(Assembly.GetExecutingAssembly())
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Setup DI container
                //services.AddTransient<RecordingService>();
                services.AddTransient<TranscriptionService>();
                services.AddTransient<OpenAIClientService>();
                services.AddTransient<QueryService>();
                services.AddTransient<OutputService>();
                services.AddHostedService<RecordingServiceRunner>();
            })
            .Build();

        await host.RunAsync();
    }
}
