using System;
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
            _config = builder.AddJsonFile("appSettings.json").Build();

            _serviceCollection = new ServiceCollection();

            _serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder.AddConfiguration(_config.GetSection("Logging")).AddConsole());
            _serviceCollection.Configure<CsvDataFileConfiguration>(_config.GetSection("CsvDataFileConfiguration"));
            _serviceCollection.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<CsvDataFileConfiguration>>().Value);
            _serviceCollection.AddSingleton(_config);

            _serviceCollection.AddSingleton<ICsvDataFileService, CsvDataFileService>();
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

            var overrideConfig = ServiceProvider.GetService<CsvDataFileConfiguration>();

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
       
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            await scoreReportService.ExportScoreReportToFile(@"D:\ACA_exercise.txt");
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
