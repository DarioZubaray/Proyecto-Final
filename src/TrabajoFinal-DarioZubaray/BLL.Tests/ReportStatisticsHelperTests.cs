using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;

namespace BLL.Tests
{
    [TestClass]
    public class ReportStatisticsHelperTests
    {
        #region Tests - Compute
        [TestMethod]
        public void Compute_Null_ReturnsEmptyStatistics()
        {
            ReportStatisticsBE statistics = ReportStatisticsHelper.Compute(null);

            Assert.AreEqual(0, statistics.TotalStudents);
            Assert.AreEqual(0, statistics.StudentsWithGrades);
            Assert.IsNull(statistics.GeneralAverage);
            Assert.AreEqual(0, statistics.Approved);
            Assert.AreEqual(0, statistics.Failed);
            Assert.IsNull(statistics.AverageAttendance);
        }

        [TestMethod]
        public void Compute_EmptyList_ReturnsEmptyStatistics()
        {
            ReportStatisticsBE statistics = ReportStatisticsHelper
                .Compute(new List<StudentPerformanceBE>());

            Assert.AreEqual(0, statistics.TotalStudents);
            Assert.IsNull(statistics.GeneralAverage);
        }

        [TestMethod]
        public void Compute_MixedData_ReturnsAggregates()
        {
            var performance = new List<StudentPerformanceBE>
            {
                new StudentPerformanceBE { StudentId = 1, Average = 8.5m, AttendancePercent = 80 },
                new StudentPerformanceBE { StudentId = 2, Average = 5.9m, AttendancePercent = 90 },
                new StudentPerformanceBE { StudentId = 3, Average = 6.0m, AttendancePercent = null },
                new StudentPerformanceBE { StudentId = 4, Average = null, AttendancePercent = 100 }
            };

            ReportStatisticsBE statistics = ReportStatisticsHelper.Compute(performance);

            Assert.AreEqual(4, statistics.TotalStudents);
            Assert.AreEqual(3, statistics.StudentsWithGrades);
            Assert.AreEqual(6.8m, statistics.GeneralAverage);
            Assert.AreEqual(2, statistics.Approved);
            Assert.AreEqual(1, statistics.Failed);
            Assert.AreEqual(90, statistics.AverageAttendance);
        }

        [TestMethod]
        public void Compute_AverageExactlyPassingGrade_IsApproved()
        {
            var performance = new List<StudentPerformanceBE>
            {
                new StudentPerformanceBE { StudentId = 1, Average = ReportStatisticsHelper.MinimumPassingGrade }
            };

            ReportStatisticsBE statistics = ReportStatisticsHelper.Compute(performance);

            Assert.AreEqual(1, statistics.Approved);
            Assert.AreEqual(0, statistics.Failed);
        }

        [TestMethod]
        public void Compute_NoGrades_AveragesNullAndCountsZero()
        {
            var performance = new List<StudentPerformanceBE>
            {
                new StudentPerformanceBE { StudentId = 1, Average = null, AttendancePercent = 70 },
                new StudentPerformanceBE { StudentId = 2, Average = null, AttendancePercent = null }
            };

            ReportStatisticsBE statistics = ReportStatisticsHelper.Compute(performance);

            Assert.AreEqual(2, statistics.TotalStudents);
            Assert.AreEqual(0, statistics.StudentsWithGrades);
            Assert.IsNull(statistics.GeneralAverage);
            Assert.AreEqual(0, statistics.Approved);
            Assert.AreEqual(0, statistics.Failed);
            Assert.AreEqual(70, statistics.AverageAttendance);
        }

        [TestMethod]
        public void Compute_GeneralAverage_IsRoundedToTwoDecimals()
        {
            var performance = new List<StudentPerformanceBE>
            {
                new StudentPerformanceBE { StudentId = 1, Average = 7.0m },
                new StudentPerformanceBE { StudentId = 2, Average = 7.0m },
                new StudentPerformanceBE { StudentId = 3, Average = 7.1m }
            };

            ReportStatisticsBE statistics = ReportStatisticsHelper.Compute(performance);

            Assert.AreEqual(7.03m, statistics.GeneralAverage);
        }
        #endregion
    }
}
