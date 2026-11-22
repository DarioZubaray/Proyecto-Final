using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IReportMPP
    {
        List<StudentPerformanceBE> GetCoursePerformance(int courseId);
        List<AttendanceReportLineBE> GetAttendanceReport(int courseId);
        List<GradeReportLineBE> GetGradesReport(int courseId);
    }
}
