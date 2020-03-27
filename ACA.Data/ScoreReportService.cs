using System.Diagnostics;
using System.IO;
using System.Linq;
using ACA.Domain;
using Microsoft.Extensions.Logging;

namespace ACA.Data
{
    public class ScoreReportService : IScoreReportService
    {
        private readonly ILogger<ScoreReportService> _logger;
        private readonly ICsvDataFileService _csvDataFileService;

        public ScoreReportService(ILogger<ScoreReportService> logger, ICsvDataFileService csvDataFileService)
        {
            _logger = logger;
            _csvDataFileService = csvDataFileService;
        }

        public ScoreReport GetScoreReport()
        {
            var stopWatch = Stopwatch.StartNew();
            _logger.LogInformation("GetScoreReport - Started");
            var scoreReport = new ScoreReport();
            var dataFiles = _csvDataFileService.GetCsvFilesInDirectory();
            _logger.LogInformation("GetScoreReport - Found {Count} Files, DataFileLocation:{DataFileLocation}, FileSearchPattern:{FileSearchPattern}",
                dataFiles.Count,_csvDataFileService.DataFileLocation,_csvDataFileService.FileSearchPattern);
            foreach (var dataFile in dataFiles)
            {
                var classScoreReport = new ClassScoreReport();
                var studentGrades = _csvDataFileService.GetStudentGradesFromCsvFile(dataFile);
                _logger.LogInformation("GetScoreReport - Read {Count} Grades From File {dataFile}", studentGrades.Count, dataFile);
                //2.	Student scores of 0 should be ignored during the calculation. - also assuming nulls should be excluded
                classScoreReport.IncludedStudents = studentGrades.
                    Where(student => student.Grade.HasValue && student.Grade.Value > 0).ToList();
                classScoreReport.TotalStudents = studentGrades.Count;
                classScoreReport.ClassName = Path.GetFileNameWithoutExtension(dataFile);
                classScoreReport.ExcludedStudents = studentGrades
                    .Where(student => !student.Grade.HasValue || student.Grade.Value == 0).ToList();
                scoreReport.ClassScores.Add(classScoreReport);
                stopWatch.Stop();
                //TODO:Instead of just logging this out, this could be a useful metric to capture to Cloudwatch or whatever monitoring is used
                _logger.LogInformation("GetScoreReport - Completed in {ElapsedMilliseconds} Milliseconds", stopWatch.ElapsedMilliseconds);
            }
            return scoreReport;
        }
    }
}