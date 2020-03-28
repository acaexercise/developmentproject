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
        public void GetScoreReportTest()
        {
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            var scoreReport = scoreReportService.GetScoreReport();
            Assert.IsNotNull(scoreReport);
        }

        [TestMethod]
        public async Task ExportScoreReportToFileTest()
        {
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            await scoreReportService.ExportScoreReportToFile(@"D:\ACA_exercise.txt");
        }

        [TestMethod]
        public async Task ExportScoreReportToFileEmptyFileDoesNOTThrowTests()
        {
            var overrideConfig = ServiceProvider.GetService<CsvDataFileConfiguration>();
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            
            overrideConfig.DataFileLocation = @"TestDataFiles";
            overrideConfig.FileSearchPattern = "Empty.csv";
            await scoreReportService.ExportScoreReportToFile(@"D:\ACA_exercise.txt");
        }

    }
}