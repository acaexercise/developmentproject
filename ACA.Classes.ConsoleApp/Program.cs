﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ACA.Classes.ConsoleApp
{
    class Program
    {
        private static IConfiguration _config;
        private static IServiceCollection _serviceCollection;

        static async Task Main(string[] args)
        {
            if (args.Length ==1 && args[0].ToLower() == "-help")
            {
                ShowHelp();
                Environment.Exit(-1);
            }

            var builder = new ConfigurationBuilder();
            _config = builder.AddJsonFile("appSettings.json").AddEnvironmentVariables().Build();

            _serviceCollection = new ServiceCollection();

            _serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder.AddConfiguration(_config.GetSection("Logging")).AddConsole());
            

            _serviceCollection.AddSingleton(_config);

            var useS3 = _config.GetValue<bool>("UseS3", false);
            if (useS3)
            {
                _serviceCollection.AddSingleton<ICsvDataFileService, S3CsvDataFileService>();
                _serviceCollection.Configure<S3CsvDataFileConfiguration>(_config.GetSection("S3CsvDataFileConfiguration"));
                _serviceCollection.AddSingleton<ICsvDataFileConfiguration>(resolver =>
                    resolver.GetRequiredService<IOptions<S3CsvDataFileConfiguration>>().Value);
            }
            else
            {
                _serviceCollection.AddSingleton<ICsvDataFileService, CsvDataFileService>();
                _serviceCollection.Configure<CsvDataFileConfiguration>(_config.GetSection("CsvDataFileConfiguration"));
                _serviceCollection.AddSingleton<ICsvDataFileConfiguration>(resolver =>
                    resolver.GetRequiredService<IOptions<CsvDataFileConfiguration>>().Value);
            }

            _serviceCollection.AddSingleton<IScoreReportService, ScoreReportService>();

            var logger = ServiceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //run validation here to identify invalid configurations
            var validatables = ServiceProvider.GetServices<IValidatable>();
            foreach (var validatable in validatables)
            {
                validatable.Validate();
            }

            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();

            if (Array.IndexOf(args, "-DataFileLocation") + 1 < args.Length)
            {
                logger.LogInformation("DataFileLocation Provided - overriding default location");
                overrideConfig.DataFileLocation = args[Array.IndexOf(args, "-DataFileLocation") + 1];
            }
            else
            {
                logger.LogInformation("No DataFileLocation Provided - using default location");
            }
            if (Array.IndexOf(args, "-FileSearchPattern") + 1 < args.Length)
            {
                logger.LogInformation("FileSearchPattern Provided - overriding default pattern");
                overrideConfig.FileSearchPattern = args[Array.IndexOf(args, "-FileSearchPattern") + 1];
            }
            else
            {
                logger.LogInformation("No FileSearchPattern Provided - using default pattern");
            }
            if (Array.IndexOf(args, "-OutputFile") + 1 < args.Length)
            {
                logger.LogInformation("OutputFile Provided - overriding default output file");
                overrideConfig.FileSearchPattern = args[Array.IndexOf(args, "-OutputFile") + 1];
            }
            else
            {
                logger.LogInformation("No FileSearchPattern Provided - using default pattern");
            }
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            var stream = await scoreReportService.ExportScoreReportToStreamAsync();
            //await scoreReportService.ExportScoreReportToFileAsync(overrideConfig.OutputFileFolder + Guid.NewGuid().ToString() + ".txt");
        }

        private static IServiceProvider _serviceProvider;
        protected static IServiceProvider ServiceProvider => _serviceProvider ??= _serviceCollection.BuildServiceProvider();

        private static void ShowHelp()
        {
            //TODO:Display command Options
            Console.WriteLine("Help");
        }
    }
}
