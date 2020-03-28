using System.Threading.Tasks;
using ACA.Domain;

namespace ACA.Data
{
    public interface IScoreReportService
    {
        ScoreReport GetScoreReport();

        Task ExportScoreReportToFile(string file);
    }
}