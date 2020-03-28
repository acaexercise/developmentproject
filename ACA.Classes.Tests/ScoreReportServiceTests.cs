using System.IO;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACA.Classes.Tests
{
    [TestClass]
    public class ScoreReportServiceTests : TestBase
    {
        [TestMethod]
        public async Task GetScoreReportTest()
        {
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            var scoreReport = await scoreReportService.GetScoreReportAsync();
            Assert.IsNotNull(scoreReport);
        }

        [TestMethod]
        public async Task ExportScoreReportToFileTest()
        {
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            await scoreReportService.ExportScoreReportToFileAsync(@"D:\ACA_exercise.txt");
        }

        [TestMethod]
        public async Task ExportScoreReportToFileEmptyFileDoesNOTThrowTests()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "Empty.csv";
            await scoreReportService.ExportScoreReportToFileAsync(@"D:\ACA_exercise.txt");
        }

    }
}