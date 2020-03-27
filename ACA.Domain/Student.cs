using System;
using CsvHelper.Configuration.Attributes;

namespace ACA.Domain
{
    public class Student
    {
        [Name("Student Name")] 
        public string Name { get; set; }

        public decimal? Grade { get; set; }

        /// <summary>
        /// 3.	Student scores with a decimal should be truncated to the nearest whole number before calculating class average 
        ///         a.  For example, 99.9 should be truncated to 99
        /// </summary>
        public decimal? TruncatedGrade => Grade.HasValue ? Math.Truncate(Grade.Value) : Grade;
    }
}
