﻿using System;

namespace ACA.Domain
{
    public class CsvDataFileConfiguration: IValidatable, ICsvDataFileConfiguration
    {
        public string DataFileLocation { get; set; }

        public string FileSearchPattern { get; set; }

        public string OutputFileFolder { get; set; }

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

            if (string.IsNullOrEmpty(OutputFileFolder))
            {
                throw new Exception("OutputFileFolder.FileSearchPattern must not be null or empty");
            }
        }
    }
}