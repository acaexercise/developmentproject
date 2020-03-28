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
            var fileService = ServiceProvider.GetService<ICsvDataFileService>();
            var stream = await scoreReportService.ExportScoreReportToStreamAsync();
            var savedFile = await fileService.SaveStreamToFile(stream);
            Assert.IsNotNull(savedFile);
        }

        [TestMethod]
        public async Task ExportScoreReportToFileEmptyFileDoesNOTThrowTest()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            var scoreReportService = ServiceProvider.GetService<IScoreReportService>();
            var fileService = ServiceProvider.GetService<ICsvDataFileService>();
            
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "Empty.csv";
            var stream = await scoreReportService.ExportScoreReportToStreamAsync();
            var savedFile = await fileService.SaveStreamToFile(stream);
            Assert.IsNotNull(savedFile);
        }

    }
}