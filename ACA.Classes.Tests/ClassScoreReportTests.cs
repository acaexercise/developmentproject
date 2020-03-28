using ACA.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACA.Classes.Tests
{
    [TestClass]
    public class ClassScoreReportTests : TestBase
    {
        /// <summary>
        /// 4.	All class averages should be rounded to one decimal place
        ///       a.For example, 88.3342 should be 88.3
        /// </summary>
        [TestMethod]
        public void ShouldRoundClassAverageTest()
        {
            var classScoreReport = new ClassScoreReport();
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = 88.3342m
            });
            //3.	Student scores with a decimal should be truncated to the nearest whole number before calculating class average 
            //4.	All class averages should be rounded to one decimal place (because of 3. - this is meaningless)
            Assert.IsTrue(classScoreReport.RoundedClassAverage == 88.0m);
        }

        [TestMethod]
        public void AverageShouldBeNullIfNoStudentsTest()
        {
            var classScoreReport = new ClassScoreReport();
            Assert.IsTrue(classScoreReport.RoundedClassAverage == null);
        }

        [TestMethod]
        public void AverageShouldBeNullIfOnlyNullGradesTest()
        {
            var classScoreReport = new ClassScoreReport();
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = null
            });
            Assert.IsTrue(classScoreReport.RoundedClassAverage == null);
        }

        [TestMethod]
        public void AverageShouldBeNullIfAllGradesAreZeroTest()
        {
            var classScoreReport = new ClassScoreReport();
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = 0
            });
            Assert.IsTrue(classScoreReport.RoundedClassAverage == null);
        }

        /// <summary>
        /// 2.	Student scores of 0 should be ignored during the calculation
        /// </summary>
        [TestMethod]
        public void RoundedClassAverageIgnoresNullGradesTest()
        {
            var classScoreReport = new ClassScoreReport();
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = 88.3342m
            });
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = null
            });
            //3.	Student scores with a decimal should be truncated to the nearest whole number before calculating class average 
            //4.	All class averages should be rounded to one decimal place 
            Assert.IsTrue(classScoreReport.RoundedClassAverage == 88.0m);
        }

        /// <summary>
        /// 2.	Student scores of 0 should be ignored during the calculation
        /// </summary>
        [TestMethod]
        public void RoundedClassAverageIgnoresGradesOf0Test()
        {
            var classScoreReport = new ClassScoreReport();
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = 88.3342m
            });
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade = 0
            });
            //3.	Student scores with a decimal should be truncated to the nearest whole number before calculating class average 
            //4.	All class averages should be rounded to one decimal place (because of 3. - this is meaningless)
            Assert.IsTrue(classScoreReport.RoundedClassAverage == 88.0m);
        }

        [TestMethod]
        public void RoundedClassAverageDoesNOTThrowWhenClassAverageIsNull()
        {
            var classScoreReport = new ClassScoreReport();
            classScoreReport.IncludedStudents.Add(new Student()
            {
                Grade =null
            });
            Assert.IsTrue(classScoreReport.RoundedClassAverage == null);
        }
    }
}