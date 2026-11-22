using System.Collections.Generic;
using System.Linq;

using BE.Entities;

namespace BLL.Helpers
{
    public static class ReportStatisticsHelper
    {
        #region Propiedades
        public const decimal MinimumPassingGrade = 6m;
        #endregion

        #region Métodos
        public static ReportStatisticsBE Compute(IList<StudentPerformanceBE> performance)
        {
            var statistics = new ReportStatisticsBE();

            if (performance == null || performance.Count == 0)
            {
                return statistics;
            }

            statistics.TotalStudents = performance.Count;

            List<StudentPerformanceBE> withGrades = performance
                .Where(p => p.Average.HasValue)
                .ToList();

            statistics.StudentsWithGrades = withGrades.Count;

            if (withGrades.Count > 0)
            {
                statistics.GeneralAverage = System.Math.Round(
                    withGrades.Average(p => p.Average.Value), 2);

                statistics.Approved = withGrades
                    .Count(p => p.Average.Value >= MinimumPassingGrade);
                statistics.Failed = statistics.StudentsWithGrades - statistics.Approved;
            }

            List<int> attendance = performance
                .Where(p => p.AttendancePercent.HasValue)
                .Select(p => p.AttendancePercent.Value)
                .ToList();

            if (attendance.Count > 0)
            {
                statistics.AverageAttendance = (int)System.Math.Round(attendance.Average());
            }

            return statistics;
        }
        #endregion
    }
}
