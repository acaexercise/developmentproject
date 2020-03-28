using System;
using ACA.Data;
using ACA.Domain;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ACA.Classes.Tests
{
    public class TestBase
    {
        protected readonly IConfiguration Config;
        protected readonly IServiceCollection ServiceCollection;

        public TestBase()
        {
            var builder = new ConfigurationBuilder();
            Config = builder.AddJsonFile("appSettings.json").AddEnvironmentVariables().Build();
  
            ServiceCollection = new ServiceCollection();

            ServiceCollection.AddLogging(loggingBuilder => 
                loggingBuilder.AddConfiguration(Config.GetSection("Logging")).AddConsole());
    
            ServiceCollection.AddSingleton(Config);

            var useS3 = Config.GetValue<bool>("UseS3", false);
            if (useS3)
            {
                ServiceCollection.AddSingleton<ICsvDataFileService, S3CsvDataFileService>();
                ServiceCollection.Configure<S3CsvDataFileConfiguration>(Config.GetSection("S3CsvDataFileConfiguration"));
                ServiceCollection.AddSingleton<ICsvDataFileConfiguration>(resolver =>
                    resolver.GetRequiredService<IOptions<S3CsvDataFileConfiguration>>().Value);
            }
            else
            {
                ServiceCollection.AddSingleton<ICsvDataFileService, CsvDataFileService>();
                ServiceCollection.Configure<CsvDataFileConfiguration>(Config.GetSection("CsvDataFileConfiguration"));
                ServiceCollection.AddSingleton<ICsvDataFileConfiguration>(resolver =>
                    resolver.GetRequiredService<IOptions<CsvDataFileConfiguration>>().Value);

            }

            ServiceCollection.AddSingleton<IScoreReportService, ScoreReportService>();
 
            //run validation here to identify invalid configurations
            var validatables = ServiceProvider.GetServices<IValidatable>();
            foreach (var validatable in validatables)
            {
                validatable.Validate();
            }
        }

        private IServiceProvider _serviceProvider;
        protected IServiceProvider ServiceProvider => _serviceProvider ??= ServiceCollection.BuildServiceProvider();
    }
}