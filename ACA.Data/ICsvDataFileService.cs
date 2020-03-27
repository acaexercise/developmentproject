using System.Collections.Generic;
using ACA.Domain;

namespace ACA.Data
{
    public interface ICsvDataFileService
    {
        List<string> GetCsvFilesInDirectory();
 
        List<Student> GetStudentGradesFromCsvFile(string file);

        string DataFileLocation { get; }

        string FileSearchPattern { get; }
    }
}