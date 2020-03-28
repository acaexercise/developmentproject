using System;

namespace ACA.Domain
{
    public class S3CsvDataFileConfiguration : IValidatable, ICsvDataFileConfiguration
    {
        public string DataFileLocation { get; set; }

        public string FileSearchPattern { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(DataFileLocation))
            {
                throw new Exception("S3CsvDataFileConfiguration.DataFileLocation must not be null or empty");
            }

            if (string.IsNullOrEmpty(FileSearchPattern))
            {
                throw new Exception("S3CsvDataFileConfiguration.FileSearchPattern must not be null or empty");
            }
        }
    }
}