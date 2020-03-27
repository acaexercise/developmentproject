using System.Collections.Generic;
using System.Linq;

namespace ACA.Domain
{
    public class ScoreReport
    {
        public ScoreReport()
        {
            ClassScores = new List<ClassScoreReport>();
        }

        /// <summary>
        /// 5.	The output file should contain the following:
        /// a.	The highest performing class (output in a prominent way)
        /// </summary>
        public string HighestPerformingClass 
        {
            get
            {
                var bestInClass = ClassScores.Where(report => report.ClassAverage.HasValue)
                    .OrderByDescending(report => report.ClassAverage.Value).FirstOrDefault();
                return bestInClass?.ClassName;
            }
        }

        /// <summary>
        /// 5.	The output file should contain the following:
        /// b.	The average score for all students regardless of class 
        /// </summary>
        public decimal? AverageAllStudents
        {
            get
            {
                var allStudents = ClassScores.SelectMany(report => report.IncludedStudents)
                    .Average(student => student.TruncatedGrade);
                return allStudents;
            }
        }

        public List<ClassScoreReport> ClassScores { get; set; }
    }
}