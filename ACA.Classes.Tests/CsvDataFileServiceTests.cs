using System.Runtime.InteropServices.ComTypes;
using ACA.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACA.Classes.Tests
{
    [TestClass]
    public class CsvDataFileServiceTests: TestBase
    {
        [TestMethod]
        public void GetCsvFilesInDirectoryTest()
        {
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = csvDataFileService.GetCsvFilesInDirectory();
            Assert.IsNotNull(dataFiles);
        }

        [TestMethod]
        public void GetStudentGradesFromCsvFileTest()
        {
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = csvDataFileService.GetCsvFilesInDirectory();
            foreach (var dataFile in dataFiles)
            {
                var students = csvDataFileService.GetStudentGradesFromCsvFile(dataFile);
                Assert.IsNotNull(students);
            }
        }
    }
}
