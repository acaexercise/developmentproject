using System.Collections.Generic;
using System.Threading.Tasks;
using ACA.Domain;

namespace ACA.Data
{
    public interface ICsvDataFileService
    {
        Task<List<string>> GetCsvFilesInDirectoryAsync();

        Task<List<Student>> GetStudentGradesFromCsvFileAsync(string file);

        string DataFileLocation { get; }

        string FileSearchPattern { get; }
    }
}