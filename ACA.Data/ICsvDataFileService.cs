using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ACA.Domain;

namespace ACA.Data
{
    public interface ICsvDataFileService
    {
        Task<List<string>> GetCsvFilesInDirectoryAsync();

        Task<bool> ResetDataFilesAsync();

        Task<List<Student>> GetStudentGradesFromCsvFileAsync(string file);

        Task<string> SaveStreamToFile(Stream memoryStream, string folder = null,string fileName=null);

        string DataFileLocation { get; }

        string FileSearchPattern { get; }
    }
}