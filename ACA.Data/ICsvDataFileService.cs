using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ACA.Domain;

namespace ACA.Data
{
    public interface ICsvDataFileService
    {
        Task<List<string>> GetCsvFilesInDirectoryAsync();

        Task<List<Student>> GetStudentGradesFromCsvFileAsync(string file);

        Task<string> SaveStreamToFile(Stream memoryStream);

        string DataFileLocation { get; }

        string FileSearchPattern { get; }
    }
}