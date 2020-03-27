using ACA.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACA.Classes.Tests
{
    [TestClass]
    public class StudentTests : TestBase
    {
        /// <summary>
        /// 3.	Student scores with a decimal should be truncated to the nearest whole number before calculating class average 
        ///         a.  For example, 99.9 should be truncated to 99
        /// </summary>
        [TestMethod]
        public void ShouldTruncateGradeTest()
        {
            var student = new Student();
            student.Grade = 99.9m;
            Assert.IsTrue(student.TruncatedGrade == 99);
        }

        [TestMethod]
        public void TruncatedGradeDoesNOTThrowWhenGradeIsNull()
        {
            var student = new Student();
            Assert.IsTrue(student.TruncatedGrade == null);
        }
    }
}