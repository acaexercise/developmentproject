using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using Microsoft.AspNetCore.Components;

namespace ACA.Classes.Blazor.Pages
{
    public class ClassesBase: ComponentBase
    {
        public List<ClassScoreReport> ClassScoreReports { get; set; }

        [Inject] protected IScoreReportService ScoreReportService { get; set; }

        public bool _isLoaded;

        protected override async Task OnInitializedAsync()
        {
            if (_isLoaded)
            {
                return;
            }
            await LoadScoreReportsData();
            _isLoaded = true;
        }

        protected virtual async Task LoadScoreReportsData()
        {
            var scoreReport = await ScoreReportService.GetScoreReportAsync();
            ClassScoreReports = scoreReport.ClassScores.Where(c => c.RoundedClassAverage.HasValue)
                .OrderByDescending(c => c.RoundedClassAverage.Value).ToList();

        }
    }
}
