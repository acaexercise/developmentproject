using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using ACA.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACA.Classes.Tests
{
    [TestClass]
    public class CsvDataFileServiceTests: TestBase
    {
        [TestMethod]
        public async Task GetCsvFilesInDirectoryTest()
        {
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsNotNull(dataFiles);
        }

        [TestMethod]
        public async Task GetStudentGradesFromCsvFileTest()
        {
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            foreach (var dataFile in dataFiles)
            {
                var students = await csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFile);
                Assert.IsNotNull(students);
            }
        }
    }
}
