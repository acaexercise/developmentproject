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
            Config = builder.AddJsonFile("appSettings.json").Build();
  
            ServiceCollection = new ServiceCollection();

            ServiceCollection.AddLogging(loggingBuilder => 
                loggingBuilder.AddConfiguration(Config.GetSection("Logging")).AddConsole());
            ServiceCollection.Configure<CsvDataFileConfiguration>(Config.GetSection("CsvDataFileConfiguration"));
            ServiceCollection.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<CsvDataFileConfiguration>>().Value);
            ServiceCollection.AddSingleton(Config);

            ServiceCollection.AddSingleton<ICsvDataFileService, CsvDataFileService>();
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