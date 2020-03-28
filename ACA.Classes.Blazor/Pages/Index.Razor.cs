using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ACA.Classes.Blazor.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] protected IScoreReportService ScoreReportService { get; set; }
        [Inject] protected ICsvDataFileService CsvDataFileService { get; set; }
        [Inject] protected IJSRuntime Js { get; set; }
        [Inject] protected ICsvDataFileConfiguration CsvDataFileConfiguration { get; set; }

        public List<ClassScoreReport> ClassScoreReports { get; set; }

        public string[] Categories { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadScoreReportsData();
        }

        protected virtual async Task LoadScoreReportsData()
        {
            var scoreReport = await ScoreReportService.GetScoreReportAsync();
            ClassScoreReports = scoreReport.ClassScores.Where(c=> c.RoundedClassAverage.HasValue).
                OrderByDescending(c => c.RoundedClassAverage.Value).ToList();
            Categories = ClassScoreReports.Select(s => s.ClassName).ToArray();
        }

        protected async Task OnClickHandler()
        {
            try
            {
                var stream = await ScoreReportService.ExportScoreReportToStreamAsync();
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                await Js.InvokeAsync<string>("FileSaveAs",  Guid.NewGuid(), text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
