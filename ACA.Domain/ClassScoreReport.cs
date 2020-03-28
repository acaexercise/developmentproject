using System;
using System.Collections.Generic;
using System.Linq;

namespace ACA.Domain
{
    public class ClassScoreReport
    {
        public ClassScoreReport()
        {
            ExcludedStudents = new List<Student>();
            IncludedStudents = new List<Student>();
        }

        public string ClassName { get; set; }

        /// <summary>
        /// c.	For each class:
        /// i.	Average score for the class
        /// </summary>
        public decimal? ClassAverage => IncludedStudents.
            Any(student => student.TruncatedGrade.HasValue && student.TruncatedGrade.Value > 0)? IncludedStudents.
            Where(student => student.TruncatedGrade.HasValue && student.TruncatedGrade.Value > 0).
            Average(student => student.TruncatedGrade.Value):(decimal?) null;

        /// <summary>
        /// 4.	All class averages should be rounded to one decimal place
        ///       a.For example, 88.3342 should be 88.3
        /// </summary>
        public decimal? RoundedClassAverage => ClassAverage.HasValue ? 
            Math.Round(ClassAverage.Value,1) : ClassAverage;

        /// <summary>
        /// c.	For each class:
        /// ii.	Total number of students within the class
        /// </summary>
        public int TotalStudents { get; set; }

        /// <summary>
        /// c.	For each class:
        /// iii.	The number of students used to calculate the class average
        /// </summary>
        public int TotalStudentsIncludedInAverage => IncludedStudents.Count;

        public int TotalStudentsExcludedFromAverage => ExcludedStudents.Count;

        /// <summary>
        /// c.	For each class:
        /// iv.	The names of any students who were discarded from consideration
        /// </summary>
        public List<Student> ExcludedStudents { get; set; }

        public List<Student> IncludedStudents { get; set; }
    }
}