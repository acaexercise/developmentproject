using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACA.Domain;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;

namespace ACA.Data
{
    public class S3CsvDataFileService : ICsvDataFileService
    {
        private readonly ILogger<S3CsvDataFileService> _logger;
        private readonly ICsvDataFileConfiguration _s3CsvDataFileConfiguration;
        private readonly IAmazonS3 _amazonS3Client;

        public S3CsvDataFileService(ILogger<S3CsvDataFileService> logger, ICsvDataFileConfiguration s3CsvDataFileConfiguration)
        {
            _logger = logger;
            _s3CsvDataFileConfiguration = s3CsvDataFileConfiguration;
            _amazonS3Client = new AmazonS3Client(RegionEndpoint.USEast2);
        }

        public string DataFileLocation => _s3CsvDataFileConfiguration.DataFileLocation;

        public string FileSearchPattern => _s3CsvDataFileConfiguration.FileSearchPattern;

        public async Task<List<string>> GetCsvFilesInDirectoryAsync()
        {
            _logger.LogInformation("GetCsvFilesInDirectory - DataFileLocation:{DataFileLocation},FileSearchPattern:{FileSearchPattern}", DataFileLocation, FileSearchPattern);

            var request = new ListObjectsV2Request
            {
                BucketName = DataFileLocation,
                MaxKeys = 1000
            };
            var s3Keys = new List<string>();
            ListObjectsV2Response response;
            do
            {
                response = await _amazonS3Client.ListObjectsV2Async(request);
                foreach (var entry in response.S3Objects.
                    Where(entry => entry.Key.IndexOf(FileSearchPattern) >= 0))
                {
                    s3Keys.Add(entry.Key);
                    _logger.LogInformation("key = {0} size = {1}", entry.Key, entry.Size);
                }
                _logger.LogInformation("Next Continuation Token: {0}", response.NextContinuationToken);
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            _logger.LogInformation("GetCsvFilesInDirectory Found {CountCsvFiles} Files Matching Search Pattern - DataFileLocation:{DataFileLocation},FileSearchPattern:{FileSearchPattern}", s3Keys.Count, DataFileLocation, FileSearchPattern);
            return s3Keys;
        }

        public async Task<List<Student>> GetStudentGradesFromCsvFileAsync(string file)
        {
            _logger.LogInformation("GetStudentGradesFromCsvFile - File:{file}", file);
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = DataFileLocation,
                    Key = file
                };
                using (var response = await _amazonS3Client.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    if (reader.BaseStream.Length == 0)
                    {
                        _logger.LogWarning("GetStudentGradesFromCsvFile - Empty File Discovered at - {file}", file);
                        return new List<Student>();
                    }

                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Student>().ToList();
                        _logger.LogInformation("GetStudentGradesFromCsvFile - Read {Count} from File:{file}",
                            records.Count(), file);
                        return records;
                    }
                }
            }
            catch (HeaderValidationException e)
            {
                _logger.LogError("GetStudentGradesFromCsvFile - Invalid Header found on file {file} - {e}", file, e);
                throw;
            }
            catch (BadDataException e)
            {
                _logger.LogError("GetStudentGradesFromCsvFile - Invalid Data found on file {file} - {e}", file, e);
                throw;
            }
            catch (TypeConverterException e)
            {
                _logger.LogError(
                    "GetStudentGradesFromCsvFile - Invalid Data (could not convert type) found on file {file} - {e}",
                    file, e);
                throw;
            }
        }
    }
}