using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace ACA.Classes.Blazor.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] protected IScoreReportService ScoreReportService { get; set; }
        [Inject] protected ICsvDataFileService CsvDataFileService { get; set; }
        [Inject] protected IJSRuntime Js { get; set; }
        [Inject] protected ICsvDataFileConfiguration CsvDataFileConfiguration { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        public List<ClassScoreReport> ClassScoreReports { get; set; }

        public string ErrorMessage { get; set; }
        public string SaveUrl => ToAbsoluteUrl("api/upload/save");
        public string RemoveUrl => ToAbsoluteUrl("api/upload/remove");
        public bool ShowError { get; set; }
        public string ToAbsoluteUrl(string url)
        {
            return $"{NavigationManager.BaseUri}{url}";
        }

        public string[] Categories { get; set; }
        
        protected async Task OnSuccessHandler(UploadSuccessEventArgs e)
        {
            await LoadScoreReportsData();
        }

        protected Task ErrorWindowVisibleChanged(bool state)
        {
            ShowError = state;
            return Task.CompletedTask;
        }

        protected Task OnErrorHandler(UploadErrorEventArgs e)
        {
            ShowError = true;
            ErrorMessage = e.Request.ResponseText;
            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadScoreReportsData();
        }

        protected virtual async Task LoadScoreReportsData()
        {
            var scoreReport = await ScoreReportService.GetScoreReportAsync();
            ClassScoreReports = scoreReport.ClassScores.Where(c => c.RoundedClassAverage.HasValue)
                .OrderByDescending(c => c.RoundedClassAverage.Value).ToList();
            Categories = ClassScoreReports.Select(s => s.ClassName).ToArray();
        }

        protected async Task OnClickHandler()
        {
            var stream = await ScoreReportService.ExportScoreReportToStreamAsync();
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            await Js.InvokeAsync<string>("FileSaveAs", Guid.NewGuid(), text);
        }
    }
}
