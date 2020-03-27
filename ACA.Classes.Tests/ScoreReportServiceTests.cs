using ACA.Data;
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
    }
}