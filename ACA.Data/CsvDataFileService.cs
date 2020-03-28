using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACA.Domain;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ACA.Data
{
    public class CsvDataFileService : ICsvDataFileService
    {
        private readonly ILogger<CsvDataFileService> _logger;
        private readonly ICsvDataFileConfiguration _csvDataFileConfiguration;

        public CsvDataFileService(ILogger<CsvDataFileService> logger, ICsvDataFileConfiguration csvDataFileConfiguration)
        {
            _logger = logger;
            _csvDataFileConfiguration = csvDataFileConfiguration;
        }

        /// <summary>
        /// Directory to read Data Files From
        /// </summary>
        public string DataFileLocation => _csvDataFileConfiguration.DataFileLocation;

        /// <summary>
        /// Pattern to use to identify data files to include
        /// </summary>
        public string FileSearchPattern => _csvDataFileConfiguration.FileSearchPattern;

        /// <summary>
        /// Returns list of all files in the configured folder (Configuration Key = DataFileLocation)
        /// matching the search pattern specified in configuration (Configuration Key FileSearchPattern)
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> GetCsvFilesInDirectoryAsync()
        {
            _logger.LogInformation("GetCsvFilesInDirectory - DataFileLocation:{DataFileLocation},FileSearchPattern:{FileSearchPattern}", DataFileLocation,FileSearchPattern);
            var csvFiles = Directory.GetFiles(DataFileLocation, FileSearchPattern);
            _logger.LogInformation("GetCsvFilesInDirectory Found {CountCsvFiles} Files Matching Search Pattern - DataFileLocation:{DataFileLocation},FileSearchPattern:{FileSearchPattern}", csvFiles.Length,DataFileLocation, FileSearchPattern);
            return Task.FromResult(csvFiles.ToList());
        }

        /// <summary>
        /// Returns a list of Students from the specified
        /// csv file
        /// </summary>
        /// <param name="file">Csv File Containing Students data,
        /// file is expected to have two columns with the
        /// first row containing Column Headers - Header columns
        /// Must be Named 'Student Name' and 'Grade'</param>
        /// <returns></returns>
        public Task<List<Student>> GetStudentGradesFromCsvFileAsync(string file)
        {
            _logger.LogInformation("GetStudentGradesFromCsvFile - File:{file}", file);
            try
            {
                using (var reader = new StreamReader(file))
                {
                    if (reader.BaseStream.Length == 0)
                    {
                        _logger.LogWarning("GetStudentGradesFromCsvFile - Empty File Discovered at - {file}",file);
                        return Task.FromResult(new List<Student>());
                    }
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Student>().ToList();
                        _logger.LogInformation("GetStudentGradesFromCsvFile - Read {Count} from File:{file}",records.Count(), file);
                        return Task.FromResult(records);
                    }
                }
            }
            //TODO:might want to track these as custom metrics/events
            catch (HeaderValidationException e)
            {
                _logger.LogError("GetStudentGradesFromCsvFile - Invalid Header found on file {file} - {e}", file,e);
                throw;
            }
            catch (BadDataException e)
            {
                _logger.LogError("GetStudentGradesFromCsvFile - Invalid Data found on file {file} - {e}", file, e);
                throw;
            }
            catch (TypeConverterException e)
            {
                _logger.LogError("GetStudentGradesFromCsvFile - Invalid Data (could not convert type) found on file {file} - {e}", file, e);
                throw;
            }
        }

        /// <summary>
        /// Saves specified memory stream to the files system
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        public Task<string> SaveStreamToFile(Stream memoryStream)
        {
            var fileName = _csvDataFileConfiguration.OutputFileFolder + Guid.NewGuid() + ".txt";
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                memoryStream.CopyTo(fs);
                fs.Flush();
            }

            return Task.FromResult(fileName);
        }
    }
}
