using System;
using System.Linq;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACA.Classes.Tests
{
    [TestClass]
    public class DataFileTests : TestBase
    {
        /// <summary>
        /// Should Consider if an empty file should throw exception or not
        /// Current setup will log a warning but won't throw
        /// </summary>
        [TestMethod]
        public async Task GetStudentGradesFromEmptyCsvFileDoesNOTThrow()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "empty.csv";
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsTrue(dataFiles.Count == 1);
            var emptyResults = csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFiles.First());
            Assert.IsNotNull(dataFiles);
        }

        /// <summary>
        /// Some Names legitimately have single quotes in them
        /// make sure this doesn't cause an issue
        /// </summary>
        [TestMethod]
        public async Task GetStudentGradesFromCsvThatContainsSingleQuoteInNameDoesNOTThrow()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "single-quote-in-name.csv";
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsTrue(dataFiles.Count == 1);
            var singleQuoteResults = await csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFiles.First());
            Assert.IsNotNull(singleQuoteResults);
            Assert.IsTrue(singleQuoteResults.Any());
            Assert.IsTrue(singleQuoteResults.First().Name == "Tammy O'Neil");

        }

        /// <summary>
        /// Double quotes generally aren't used in names
        /// TODO:its possible we could add a cleaning step to strip out the doublequote or possible ignore it
        /// but weems like this probably warrants an exception
        /// </summary>
        [TestMethod]
        public async Task GetStudentGradesFromCsvThatContainsDoubleQuoteInNameDoesNOTThrow()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "double-quote-in-name.csv";
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsTrue(dataFiles.Count == 1);
            await Assert.ThrowsExceptionAsync<BadDataException>(async () =>
            {
                var doubleQuoteResults = await csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFiles.First());
            });
        }

        /// <summary>
        /// If csv file is missing headers, we should get HeaderValidationException
        /// TODO:should be able to handle headerless file and just assume based on ordinal position
        /// </summary>
        [TestMethod]
        public async Task GetStudentGradesFromHeaderlessCsvThrowsException()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "missing-headers.csv";
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsTrue(dataFiles.Count == 1);
            await Assert.ThrowsExceptionAsync<HeaderValidationException>(async () =>
            {
                var emptyResults = await csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFiles.First());
            });
        }

        /// <summary>
        /// If csv file has incorrect headers, we should get HeaderValidationException
        /// TODO:should be able to handle (ignore) incorrect headers and just assume based on ordinal position
        /// </summary>
        [TestMethod]
        public async Task GetStudentGradesFromIncorrectHeaderCsvThrowsException()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "incorrectly-named-headers.csv";
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsTrue(dataFiles.Count == 1);
            await Assert.ThrowsExceptionAsync<HeaderValidationException>(async () =>
            {
                var emptyResults = await csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFiles.First());
            });
        }

        /// <summary>
        /// If csv file has incorrect headers, we should get HeaderValidationException
        /// TODO:should be able to handle (ignore) incorrect headers and just assume based on ordinal position
        /// </summary>
        [TestMethod]
        public async Task GetStudentGradesFromInvalidGradeFormCsvThrowsException()
        {
            var overrideConfig = ServiceProvider.GetService<ICsvDataFileConfiguration>();
            overrideConfig.DataFileLocation = @"aca-testdatafiles";
            overrideConfig.FileSearchPattern = "invalid-grade-format.csv";
            var csvDataFileService = ServiceProvider.GetService<ICsvDataFileService>();
            var dataFiles = await csvDataFileService.GetCsvFilesInDirectoryAsync();
            Assert.IsTrue(dataFiles.Count == 1);
            await Assert.ThrowsExceptionAsync<TypeConverterException>(async () =>
            {
                var emptyResults = await csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFiles.First());
            });
        }
    }
}