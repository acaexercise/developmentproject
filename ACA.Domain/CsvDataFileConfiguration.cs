using System;

namespace ACA.Domain
{
    public class CsvDataFileConfiguration: IValidatable
    {
        public string DataFileLocation { get; set; }

        public string FileSearchPattern { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(DataFileLocation))
            {
                throw new Exception("CsvDataFileConfiguration.DataFileLocation must not be null or empty");
            }

            if (string.IsNullOrEmpty(FileSearchPattern))
            {
                throw new Exception("CsvDataFileConfiguration.FileSearchPattern must not be null or empty");
            }
        }
    }
}