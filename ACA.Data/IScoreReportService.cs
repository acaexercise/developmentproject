using System.IO;
using System.Threading.Tasks;
using ACA.Domain;

namespace ACA.Data
{
    public interface IScoreReportService
    {
        Task<ScoreReport> GetScoreReportAsync();

        Task<Stream> ExportScoreReportToStreamAsync();

        Task<ClassScoreReport> GetClassScoreReportAsync(string dataFile);
    }
}