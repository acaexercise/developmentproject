using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Stream> ExportScoreReportToStreamAsync()
        {
            var scoreReport = await GetScoreReportAsync();
            var highestPerformingClassLabel = " Highest Performing Class(es) - ";
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            await streamWriter.WriteLineAsync(new string('*', 100));
            if (!string.IsNullOrEmpty(scoreReport.HighestPerformingClass))
            {
                await streamWriter.WriteLineAsync($"*** {highestPerformingClassLabel} {scoreReport.HighestPerformingClass}{new string(' ', 92 - highestPerformingClassLabel.Length - scoreReport.HighestPerformingClass.Length)}***");
            }
            else
            {
                if (!scoreReport.ClassScores.Any())
                {
                    await streamWriter.WriteLineAsync("No Score Data Found, please check the input files");
                    return memoryStream;
                }
            }
            await streamWriter.WriteLineAsync(new string('*', 100));
            await streamWriter.WriteLineAsync();
            if (scoreReport.AverageAllStudents.HasValue)
            {
                await streamWriter.WriteLineAsync($"All Students Average Score - {Math.Round(scoreReport.AverageAllStudents.Value, 1)}");
            }

            foreach (var classScore in scoreReport.ClassScores.OrderByDescending(c => c.ClassAverage))
            {
                await streamWriter.WriteLineAsync();
                await streamWriter.WriteLineAsync($"Class - {classScore.ClassName}");
                if (classScore.RoundedClassAverage.HasValue)
                {
                    await streamWriter.WriteLineAsync($"Class Score Average - {classScore.RoundedClassAverage.Value}");
                }
                await streamWriter.WriteLineAsync($"Class Total Students - {classScore.TotalStudents}");
                await streamWriter.WriteLineAsync($"Class Total Students included in Average - {classScore.IncludedStudents.Count}");
                if (classScore.ExcludedStudents.Any())
                {
                    await streamWriter.WriteLineAsync("Students Excluded");
                    foreach (var excludedStudent in classScore.ExcludedStudents)
                    {
                        await streamWriter.WriteLineAsync($"{excludedStudent.Name}");
                    }
                }
            }
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public async Task<ScoreReport> GetScoreReportAsync()
        {
            var stopWatch = Stopwatch.StartNew();
            _logger.LogInformation("GetScoreReport - Started");
            var scoreReport = new ScoreReport();
            var dataFiles = await _csvDataFileService.GetCsvFilesInDirectoryAsync();
            _logger.LogInformation("GetScoreReport - Found {Count} Files, DataFileLocation:{DataFileLocation}, FileSearchPattern:{FileSearchPattern}",
                dataFiles.Count,_csvDataFileService.DataFileLocation,_csvDataFileService.FileSearchPattern);
            foreach (var dataFile in dataFiles)
            {
                var classScoreReport = await GetClassScoreReportAsync(dataFile);
                scoreReport.ClassScores.Add(classScoreReport);
            }
            //TODO:Instead of just logging this out, this could be a useful metric to capture to Cloudwatch or whatever monitoring is used
            stopWatch.Stop();
            _logger.LogInformation("GetScoreReport - Completed in {ElapsedMilliseconds} Milliseconds", stopWatch.ElapsedMilliseconds);
            return scoreReport;
        }

        public async Task<ClassScoreReport> GetClassScoreReportAsync(string dataFile)
        {
            var classScoreReport = new ClassScoreReport();
            var studentGrades = await _csvDataFileService.GetStudentGradesFromCsvFileAsync(dataFile);
            _logger.LogInformation("GetScoreReport - Read {Count} Grades From File {dataFile}", studentGrades.Count, dataFile);
            //2.	Student scores of 0 should be ignored during the calculation. - also assuming nulls should be excluded
            classScoreReport.IncludedStudents = studentGrades.
                Where(student => student.Grade.HasValue && student.Grade.Value > 0).ToList();
            classScoreReport.TotalStudents = studentGrades.Count;
            classScoreReport.ClassName = Path.GetFileNameWithoutExtension(dataFile);
            classScoreReport.ExcludedStudents = studentGrades
                .Where(student => !student.Grade.HasValue || student.Grade.Value == 0).ToList();
            return classScoreReport;
        }
    }
}