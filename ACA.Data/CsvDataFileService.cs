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
        private readonly CsvDataFileConfiguration _csvDataFileConfiguration;

        public CsvDataFileService(ILogger<CsvDataFileService> logger,CsvDataFileConfiguration csvDataFileConfiguration)
        {
            _logger = logger;
            _csvDataFileConfiguration = csvDataFileConfiguration;
        }

        public string DataFileLocation => _csvDataFileConfiguration.DataFileLocation;

        public string FileSearchPattern => _csvDataFileConfiguration.FileSearchPattern;

        /// <summary>
        /// Returns list of all files in the configured folder (Configuration Key = DataFileLocation)
        /// matching the search pattern specified in configuration (Configuration Key FileSearchPattern)
        /// </summary>
        /// <returns></returns>
        public List<string> GetCsvFilesInDirectory()
        {
            _logger.LogInformation("GetCsvFilesInDirectory - DataFileLocation:{DataFileLocation},FileSearchPattern:{FileSearchPattern}", DataFileLocation,FileSearchPattern);
            var csvFiles = Directory.GetFiles(DataFileLocation, FileSearchPattern);
            _logger.LogInformation("GetCsvFilesInDirectory Found {CountCsvFiles} Files Matching Search Pattern - DataFileLocation:{DataFileLocation},FileSearchPattern:{FileSearchPattern}", csvFiles.Length,DataFileLocation, FileSearchPattern);
            return csvFiles.ToList();
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
        public List<Student> GetStudentGradesFromCsvFile(string file)
        {
            _logger.LogInformation("GetStudentGradesFromCsvFile - File:{file}", file);
            try
            {
                using (var reader = new StreamReader(file))
                {
                    if (reader.BaseStream.Length == 0)
                    {
                        _logger.LogWarning("GetStudentGradesFromCsvFile - Empty File Discovered at - {file}",file);
                        return new List<Student>();
                    }
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Student>();
                        _logger.LogInformation("GetStudentGradesFromCsvFile - Read {Count} from File:{file}",records.Count(), file);
                        return records.ToList();
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
    }
}
